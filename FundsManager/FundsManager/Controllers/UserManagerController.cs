using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FundsManager.DAL;
using FundsManager.Models;
using System.IO;
using FundsManager.Common;
using System.Configuration;
using FundsManager.ViewModels;
using System.Text;
using System.Data.Entity.Validation;

namespace FundsManager.Controllers
{
    public class UserManagerController : Controller
    {
        private FundsContext db = new FundsContext();

        // GET: UserManager
        public ActionResult Index()
        {
            return View(db.User_Info.ToList());
        }

        // GET: UserManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Info user_Info = db.User_Info.Find(id);
            if (user_Info == null)
            {
                return HttpNotFound();
            }
            return View(user_Info);
        }

        // GET: UserManager/Create
        public ActionResult Create()
        {
            setSelect();
            return View();
        }

        // POST: UserManager/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,realName,certificateType,certificateNo,mobile,email,password,password2,state,gender,postId,officePhone,picture,deptId,deptChild,roleId")]UserEditModel model)
        {
            setSelect();
            if (ModelState.IsValid)
            {
                User_Info info = new User_Info();
                info.ToDecrypt();
                model.toUserInfoDB(info);
                if (db.User_Info.Where(x => x.user_name == info.user_name).Count() > 0)
                {
                    ViewBag.msg = "该用户名已注册。";
                    goto next;
                }
                if (db.User_Info.Where(x => x.user_certificate_type == info.user_certificate_type && x.user_certificate_no == info.user_certificate_no).Count() > 0)
                {
                    ViewBag.msg = "该证件号已注册。";
                    goto next;
                }
                if (db.User_Info.Where(x => x.user_email == info.user_email).Count() > 0)
                {
                    ViewBag.msg = "该邮箱已注册。";
                    goto next;
                }
                if (db.User_Info.Where(x => x.user_mobile == info.user_mobile).Count() > 0)
                {
                    ViewBag.msg = "该手机号已注册。";
                    goto next;
                }
                if (model.password!=model.password2)
                {
                    ViewBag.msg = "两次输入密码不一致，请重新输入。";
                    goto next;
                }
                var salt = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
                info.user_password = PasswordUnit.getPassword(model.password, salt);
                info.user_salt = salt;
                info.ToEncrypt();
                db.User_Info.Add(info);
                try
                {
                    db.SaveChanges();
                }catch(Exception ex)
                {
                    ViewBag.msg = "信息录入失败，请重新录入。";
                    ErrorUnit.WriteErrorLog(ex.ToString(), this.GetType().Name);
                    goto next;
                }
                
                User_Extend extend = new User_Extend();
                model.toUserExtendDB(extend);
                extend.user_id = info.user_id;
                db.User_Extend.Add(extend);
                string photoDir = ConfigurationManager.AppSettings["photoPath"];
                if (!Directory.Exists(photoDir)) Directory.CreateDirectory(photoDir);
                string photoTempDir = ConfigurationManager.AppSettings["tempPhotoPath"];
                string file_name = string.Format("{0}{1}", photoDir, extend.user_picture).Replace("_temp", "");
                string temp_file_name = string.Format("{0}{1}", photoTempDir, extend.user_picture);
                if (System.IO.File.Exists(temp_file_name))
                {
                    FileInfo fi = new FileInfo(temp_file_name);
                    fi.CopyTo(file_name, true);
                }
                else ViewBag.msg = "图片保存失败。";
                if (model.roleId != null)
                {
                    User_vs_Role uvr = new User_vs_Role();
                    uvr.uvr_user_id = info.user_id;
                    uvr.uvr_role_id = (int)model.roleId;
                    db.User_vs_Role.Add(uvr);
                }
                db.SaveChanges();
            }
            else
            {
                StringBuilder sbmsg = new StringBuilder();
                foreach (var value in ModelState.Values)
                {
                    if (value.Errors.Count() > 0)
                    {
                        foreach (var err in value.Errors)
                        {
                            sbmsg.Append(err.ErrorMessage);
                        }
                        ViewBag.msg = sbmsg.ToString(); ;
                    }
                }
            }
            next:
            return View(model);
        }

        // GET: UserManager/Edit/5
        public ActionResult Edit(int? id)
        {
            setSelect();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserEditModel model = new UserEditModel();
            User_Info info = db.User_Info.Find(id);
            if (info == null)
            {
                ViewBag.msg = "该用户可能已被删除，无法查到该用户信息。";
                goto next;
            }
            info.ToDecrypt();
            model.FromUserInfoDB(info);
            User_Extend extend = db.User_Extend.Find(id);
            if (extend != null)
            {
                model.FromUserExtendDB(extend);
                int p = (from dept in db.Dic_Department where dept.dept_id == extend.user_dept_id select dept.dept_parent_id).FirstOrDefault();
                if (p == 0) model.deptId = extend.user_dept_id;
                else
                {
                    int dept_id = (from dept in db.Dic_Department where dept.dept_id == p select dept.dept_id).FirstOrDefault();
                    model.deptId = dept_id;
                    model.deptChild = extend.user_dept_id;
                    List<SelectOption> options = DropDownList.getDepartment(dept_id);
                    ViewBag.DeptChild = DropDownList.SetDropDownList(options);
                }
            }
            next:
            return View(model);
        }

        // POST: UserManager/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,realName,certificateType,certificateNo,mobile,email,password,password2,state,gender,postId,officePhone,picture,deptId,deptChild,roleId")]UserEditModel model)
        {
            setSelect();
            if (ModelState.IsValid)
            {
                if (model.deptChild != null&& model.deptId!=null)
                {
                    List<SelectOption> options = DropDownList.getDepartment((int)model.deptId);
                    ViewBag.DeptChild = DropDownList.SetDropDownList(options);
                }
                User_Info info = db.User_Info.Find(model.id);
                info.ToDecrypt();
                if (info == null)
                {
                    ViewBag.msg = "该用户可能已被删除，无法更改。";
                    goto next;
                }
                model.toUserInfoDB(info);
                if (db.User_Info.Where(x => x.user_name == info.user_name&&x.user_id!=info.user_id).Count() > 0)
                {
                    ViewBag.msg = "该用户名已注册。";
                    goto next;
                }
                if (db.User_Info.Where(x => (x.user_certificate_type == info.user_certificate_type && x.user_certificate_no == info.user_certificate_no) && x.user_id != info.user_id).Count() > 0)
                {
                    ViewBag.msg = "该证件号已注册。";
                    goto next;
                }
                if (db.User_Info.Where(x => x.user_email == info.user_email && x.user_id != info.user_id).Count() > 0)
                {
                    ViewBag.msg = "该邮箱已注册。";
                    goto next;
                }
                if (db.User_Info.Where(x => x.user_mobile == info.user_mobile && x.user_id != info.user_id).Count() > 0)
                {
                    ViewBag.msg = "该手机号已注册。";
                    goto next;
                }
                if (!string.IsNullOrEmpty(model.password))
                {
                    if (model.password != model.password2)
                    {
                        ViewBag.msg = "两次输入密码不一致，请重新输入。";
                        goto next;
                    }
                    var salt = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
                    info.user_password = PasswordUnit.getPassword(model.password, salt);
                    info.user_salt = salt;
                }
                info.ToEncrypt();
                db.Entry<User_Info>(info).State = EntityState.Modified;
                bool edit = true;
                User_Extend extend = db.User_Extend.Find(info.user_id);
                if (extend == null)
                {
                    edit = false;
                    extend = new User_Extend();
                }
                if (!string.IsNullOrEmpty(model.picture) &&model.picture != extend.user_picture)
                {
                    string photoDir = ConfigurationManager.AppSettings["photoPath"];
                    if (!Directory.Exists(photoDir)) Directory.CreateDirectory(photoDir);
                    string photoTempDir = ConfigurationManager.AppSettings["tempPhotoPath"];
                    string file_name = string.Format("{0}{1}", photoDir, model.picture).Replace("_temp", "");
                    string temp_file_name = string.Format("{0}{1}", photoTempDir, model.picture);
                    if (System.IO.File.Exists(temp_file_name))
                    {
                        FileInfo fi = new FileInfo(temp_file_name);
                        fi.CopyTo(file_name, true);
                        model.picture = Path.GetFileName(file_name);
                    }
                    else ViewBag.msg = "图片保存失败。";
                }
                model.toUserExtendDB(extend);

                if (edit)
                    db.Entry<User_Extend>(extend).State = EntityState.Modified;
                else db.User_Extend.Add(extend);
                edit = true;
                if (model.roleId != null)
                {
                    User_vs_Role uvr = db.User_vs_Role.Find(info.user_id);
                    if (uvr == null)
                    {
                        edit = false;
                        uvr = new User_vs_Role();
                    }
                    uvr.uvr_user_id = info.user_id;
                    uvr.uvr_role_id = (int)model.roleId;
                    if (edit)
                        db.Entry<User_vs_Role>(uvr).State = EntityState.Modified;
                    else db.User_vs_Role.Add(uvr);
                }
                try
                {
                    db.SaveChanges();
                }catch(DbEntityValidationException ex)
                {
                    StringBuilder errors = new StringBuilder();
                    IEnumerable<DbEntityValidationResult> validationResult = ex.EntityValidationErrors;
                    foreach (DbEntityValidationResult result in validationResult)
                    {
                        ICollection<DbValidationError> validationError = result.ValidationErrors;
                        foreach (DbValidationError err in validationError)
                        {
                            errors.Append(err.PropertyName + ":" + err.ErrorMessage +"\r\n");
                        }
                    }
                    ErrorUnit.WriteErrorLog(errors.ToString(),this.GetType().Name);
                    ViewBag.msg = " 更新失败。";
                }
            }
            next:
            return View(model);
        }

        // GET: UserManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Info user_Info = db.User_Info.Find(id);
            if (user_Info == null)
            {
                return HttpNotFound();
            }
            return View(user_Info);
        }

        // POST: UserManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User_Info user_Info = db.User_Info.Find(id);
            db.User_Info.Remove(user_Info);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult UploadPicture()
        {
            ViewModels.BaseJsonData json = new ViewModels.BaseJsonData();
            var file = Request.Files["data"];
            if (file == null)
            {
                json.state = 0;
                json.msg_text = "没有文件，请重新上传。";
            }
            if (Path.GetExtension(file.FileName).ToLower() != ".jpg")
            {
                json.state = 0;
                json.msg_text = "请上传jpg格式文件。";
            }
            string photoTempDir= ConfigurationManager.AppSettings["tempPhotoPath"];
            if (!Directory.Exists(photoTempDir)) Directory.CreateDirectory(photoTempDir);
            string guid = Guid.NewGuid().ToString("N");
            string file_name = string.Format("{0}{1}.jpg", photoTempDir, guid);
            string file_name_temp = string.Format("{0}{1}_temp.jpg",photoTempDir,guid);
            file.SaveAs(file_name);
            ImageFun.MakeThumbnail(file_name, file_name_temp, 200, 0, "W");
            json.state = 1;
            json.data = Path.GetFileName(file_name_temp); ;
            return Json(json);
        }
        public void setSelect()
        {
            List<SelectOption> options = DropDownList.getDepartment();
            ViewBag.Department = DropDownList.SetDropDownList(options);
            options = DropDownList.PostSelect();
            ViewBag.Post = DropDownList.SetDropDownList(options);
            options = DropDownList.SexSelect();
            ViewBag.Sex = DropDownList.SetDropDownList(options);
            options = DropDownList.UserStateSelect();
            ViewBag.State = DropDownList.SetDropDownList(options);
            options = DropDownList.CardTypeSelect();
            ViewBag.CardType = DropDownList.SetDropDownList(options);
            options = DropDownList.RoleSelect();
            ViewBag.Role = DropDownList.SetDropDownList(options);
        }
        [HttpPost]
        public JsonResult GetDeptChild(string id)
        {
            BaseJsonData json = new BaseJsonData();
            List<SelectOption> options = DropDownList.getDepartment(PageValidate.FilterParam(id));
            json.data = options;
            json.state = 1;
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
