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

        public ActionResult Index()
        {
            BasePagerModel pager = new BasePagerModel();
            return Index(pager);
        }
        // GET: UserManager
        [HttpPost]
        [ValidateAntiForgeryToken, wxAuthorizeAttribute(Roles = "系统管理员")]
        public ActionResult Index(BasePagerModel pager)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            if (pager == null) pager = new BasePagerModel();
            ViewData["search"] = pager;
            var list = (from user in db.User_Info
                        join uvr in db.User_vs_Role
                        on user.user_id equals uvr.uvr_user_id into T1
                        from t1 in T1.DefaultIfEmpty()
                        join role in db.Dic_Role
                        on t1.uvr_role_id equals role.role_id into T2
                        from t2 in T2.DefaultIfEmpty()
                        join ue in db.User_Extend
                        on user.user_id equals ue.user_id into T3
                        from t3 in T3.DefaultIfEmpty()
                        join dept in db.Dic_Department
                        on t3.user_dept_id equals dept.dept_id into T4
                        from t4 in T4.DefaultIfEmpty()
                        join post in db.Dic_Post
                        on t3.user_post_id equals post.post_id into T5
                        from t5 in T5.DefaultIfEmpty()
                        orderby user.user_id ascending
                        select new UserListModel
                        {
                            id = user.user_id,
                            name = user.user_name,
                            roleName = t2.role_name==null?"":t2.role_name,
                            stateTxt = user.user_state == 1 ? "正常" : (user.user_state == 2 ? "锁定" : (user.user_state == 0 ? "未启用" : "未知")),
                            realName = user.real_name,
                            times = user.user_login_times,
                            deptName=t4.dept_name==null?"":t4.dept_name,
                            postName=t5.post_name==null?"":t5.post_name,
                             picture=t3.user_picture==null?"default.jpg":t3.user_picture
                        }).Skip((pager.PageIndex-1)*pager.PageSize).Take(pager.PageSize).ToList();
            foreach(var item in list)
            {
                item.realName = Common.DEncrypt.AESEncrypt.Decrypt(item.realName);
            }
            return View(list);
        }

        // GET: UserManager/Details/5
        [wxAuthorizeAttribute(Roles = "系统管理员")]
        public ActionResult Details(int? id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            if (id == null)
            {
                id = PageValidate.FilterParam(User.Identity.Name);
            }
            User_Info user_Info = db.User_Info.Find(id);
            if (user_Info == null)
            {
                return HttpNotFound();
            }
            return View(user_Info);
        }

        // GET: UserManager/Create
        [wxAuthorizeAttribute(Roles = "系统管理员")]
        public ActionResult Create()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            setSelect();
            return View();
        }

        // POST: UserManager/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken, wxAuthorizeAttribute(Roles = "系统管理员")]
        public ActionResult Create([Bind(Include = "name,realName,certificateType,certificateNo,mobile,email,password,password2,state,gender,postId,officePhone,picture,deptId,deptChild,roleId")]UserEditModel model)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            setSelect();
            if (ModelState.IsValid)
            {
                User_Info info = new User_Info();
                model.toUserInfoDB(info);
                if (db.User_Info.Where(x => x.user_name == info.user_name).Count() > 0)
                {
                    ViewBag.msg = "该用户名已注册。";
                    goto next;
                }
                var salt = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
                info.user_password = PasswordUnit.getPassword(model.password.ToUpper(), salt);
                info.user_salt = salt;
                info.ToEncrypt();
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
                if (model.password != model.password2)
                {
                    ViewBag.msg = "两次输入密码不一致，请重新输入。";
                    goto next;
                }

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
                extend.user_add_user = PageValidate.FilterParam(User.Identity.Name);
                extend.user_add_time = DateTime.Now;
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
                ViewBag.msg = " 用户创建成功。";
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
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            LoginRole role = (LoginRole)Session["LoginRole"];
            if (role.roleName != "系统管理员") id = PageValidate.FilterParam(User.Identity.Name);
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
        [ValidateAntiForgeryToken, wxAuthorizeAttribute(Roles = "系统管理员")]
        public ActionResult Edit([Bind(Include = "id,name,realName,certificateType,certificateNo,mobile,email,password,password2,state,gender,postId,officePhone,picture,deptId,deptChild,roleId")]UserEditModel model)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            LoginRole role = (LoginRole)Session["LoginRole"];
            if (role.roleName != "系统管理员" && User.Identity.Name != model.id.ToString())
                return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限!" });
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
                if (!string.IsNullOrEmpty(model.password))
                {
                    if (model.password != model.password2)
                    {
                        ViewBag.msg = "两次输入密码不一致，请重新输入。";
                        goto next;
                    }
                    var salt = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
                    info.user_password = PasswordUnit.getPassword(model.password.ToUpper(), salt);
                    info.user_salt = salt;
                }
                info.ToEncrypt();
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
                extend.user_edit_time = DateTime.Now;
                extend.user_edit_user = PageValidate.FilterParam(User.Identity.Name);
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
            ViewBag.msg = " 更新成功。";
            return View(model);
        }

        // GET: UserManager/Delete/5
        [wxAuthorizeAttribute(Roles = "系统管理员"), HttpPost]
        public JsonResult Delete(int? id)
        {
            BaseJsonData json = new BaseJsonData();
            if(id==1) goto next;
            if (!User.Identity.IsAuthenticated) goto next;
            if (id == null) goto next;
            User_Info user_Info = db.User_Info.Find(id);
            if (user_Info == null)
            {
                json.state = 1;
                json.msg_text = "所选用户不存在或已被删除。";
                goto next;
            }
            User_Extend extend = db.User_Extend.Find(id);
            Recycle_User ru = new Recycle_User();
            ru.FromUserInfo(user_Info);
            ru.FromUserExtend(extend);
            db.User_Info.Remove(user_Info);
            if (extend != null) db.User_Extend.Remove(extend);
            db.Recycle_User.Add(ru);
            try
            {
                db.SaveChanges();
                json.state = 1;
                json.msg_code = "success";
                json.msg_text = "删除成功。";
            }
            catch(Exception ex)
            {
                ErrorUnit.WriteErrorLog(ex.ToString(), this.GetType().Name);
                json.state = 1;
                json.msg_text = "删除可能没有成功，请刷新页面查看。";
            }
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
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
