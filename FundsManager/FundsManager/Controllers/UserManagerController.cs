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
                //db.User_Info.Add(user_Info);
                //db.SaveChanges();
                //return RedirectToAction("Index");
                User_Info info = new User_Info();
                model.toUserInfoDB(info);
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
                string temp_file_name = string.Format("{0}{1}", photoDir, extend.user_picture);
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

        // POST: UserManager/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "user_id,user_name,real_name,user_certificate_type,user_certificate_no,user_mobile,user_email,user_password,user_salt,user_state,user_login_times")] User_Info user_Info)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_Info).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user_Info);
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
            DBCaches<User_Info>.ClearAllCache();
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
