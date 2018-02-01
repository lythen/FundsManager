using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FundsManager.DAL;
using FundsManager.Models;
using FundsManager.ViewModels;
using FundsManager.Common;
namespace FundsManager.Controllers
{
    public class SystemSetController : Controller
    {
        private FundsContext db = new FundsContext();

        #region 网站设置
        public ActionResult SiteInfo()
        {
            ViewModels.SiteInfo info = FundsManager.Controllers.SiteInfo.getSiteInfo();
            return View(info);
        }
        public ActionResult SiteSet()
        {
            ViewModels.SiteInfo info = FundsManager.Controllers.SiteInfo.getSiteInfo();
            return View(info);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SiteSet([Bind(Include = "name,company,introduce,companyAddress,companyPhone,companyEmail,managerName,managerPhone,managerEmail")]ViewModels.SiteInfo info)
        {

            Sys_SiteInfo model = db.Sys_SiteInfo.FirstOrDefault();
            if (model.site_name != info.name)
            {
                db.Sys_SiteInfo.Remove(model);
                info.toDBModel(model);
                db.Sys_SiteInfo.Add(model);
            }
            else
            {
                info.toDBModel(model);
                db.Entry<Sys_SiteInfo>(model).State = EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
                DBCaches<Sys_SiteInfo>.ClearCache("site-name");
                DBCaches<Sys_SiteInfo>.ClearCache("site-info");
            }
            catch(Exception ex)
            {
                @ViewBag.msg = "修改失败。";
            }
            @ViewBag.msg = "修改成功。";
            return View(info);
        }
        #endregion
        #region 模块设置
        [wxAuthorize(Roles = "系统管理员")]
        public ActionResult ContrlModule()
        {
            List<ModuleInfo> models =DBCaches2.getModuleInfo();
            foreach(ModuleInfo model in models)
            {
                int[] roles = (from rvc in db.Role_vs_Controller
                               where rvc.rvc_controller == model.name
                               select rvc.rvc_role_id
                               ).ToArray();
               RoleInfo[] rvcs = DBCaches2.getRoleInfo();
                foreach(RoleInfo item in rvcs)
                {
                    if (roles.Contains(item.id)) item.hasrole = true;
                    else item.hasrole = false;
                }
                model.roles = rvcs;
            }
            return View(models);
        }
        [HttpPost,wxAuthorize(Roles ="系统管理员")]
        public JsonResult ContrlModule(EditModules models)
        {
            BaseJsonData json = new BaseJsonData();
            if (ModelState.IsValid)
            {
                string ctrl_name;
                foreach(ModuleInfo info in models.modules)
                {
                    ctrl_name = info.name;
                    var no1 = db.Role_vs_Controller.Where(x => x.rvc_role_id != 1&& x.rvc_controller== ctrl_name);
                    if (no1.Count() > 0)
                    {
                        db.Role_vs_Controller.RemoveRange(no1);
                        db.SaveChanges();
                    }
                    if (info.roles != null && info.roles.Length > 0)
                    {
                        foreach(RoleInfo rinfo in info.roles)
                        {
                            Role_vs_Controller rvc = new Role_vs_Controller();
                            rvc.rvc_role_id = rinfo.id;
                            rvc.rvc_controller = ctrl_name;
                            if (db.Role_vs_Controller.Find(rvc.rvc_role_id,rvc.rvc_controller) == null) db.Role_vs_Controller.Add(rvc);
                        }
                    }
                }
                db.SaveChanges();
                json.state = 1;
                json.msg_code = "success";
                json.msg_text = "数据更新成功。";
                DBCaches2.ClearCache("dic-module");
            }
            else
            {
                json.msg_code = "error";
                json.msg_text = "数据接收错误。";
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 职务设置
        [wxAuthorize(Roles = "系统管理员")]
        public ActionResult Post()
        {
            ViewData["PostList"] = DBCaches<Dic_Post>.getCache("cache_post"); ;
            return View(new Dic_Post());
        }
        [HttpPost,wxAuthorize(Roles = "系统管理员")]
        [ValidateAntiForgeryToken]
        public ActionResult Post(Dic_Post model)
        {
            model.post_name = PageValidate.InputText(model.post_name, 50);
            if (db.Dic_Post.Where(x => x.post_name == model.post_name).Count() > 0) ViewBag.msg = "名称已存在";
            else
            {
                db.Dic_Post.Add(model);
                try
                {
                    db.SaveChanges();
                    DBCaches<Dic_Post>.ClearCache("cache_post");
                }
                catch
                {
                    ViewBag.msg = "职务添加失败，请重试。";
                }
            }
            ViewData["PostList"] = DBCaches<Dic_Post>.getCache("cache_post");// db.Dic_Post.ToList();
            return View(model);
        }
        [wxAuthorize(Roles ="系统管理员")]
        public JsonResult DeletePost(string pid)
        {
            int id = PageValidate.FilterParam(pid);
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                goto next;
            }
            Dic_Post model = db.Dic_Post.Find(id);
            if (model == null)
            {
                json.msg_text = "没有找到该职务，该职务可能已被删除。";
                json.msg_code = "noThis";
                goto next;
            }
            db.Dic_Post.Remove(model);
            try
            {
                db.SaveChanges();
                DBCaches<Dic_Post>.ClearCache("cache_post");
            }
            catch
            {
                json.msg_text = "删除失败，请重新操作。";
                json.msg_code = "recyErr";
                goto next;
            }
            json.state = 1;
            json.msg_code = "success";
            json.msg_text = "删除成功！";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 科室部门管理
        [wxAuthorize(Roles = "系统管理员")]
        public ActionResult Department()
        {
            List<SelectOption> options = DropDownList.getDepartment();
            ViewBag.Dept = DropDownList.SetDropDownList(options);
            ViewData["DeptList"] = DBCaches2.getDeptCache();
            return View(new DepartMentModel());
        }
        [HttpPost, wxAuthorize(Roles = "系统管理员")]
        [ValidateAntiForgeryToken]
        public ActionResult Department(DepartMentModel info)
        {
            List<SelectOption> options = DropDownList.getDepartment();
            ViewBag.Dept = DropDownList.SetDropDownList(options);
            Dic_Department model = new Dic_Department();
            model.dept_name = PageValidate.InputText(info.deptName, 50);
            if (db.Dic_Department.Where(x => x.dept_name == model.dept_name&&x.dept_parent_id==info.parentId).Count() > 0) ViewBag.msg = "名称已存在";
            else
            {
                model.dept_parent_id = info.parentId;
                db.Dic_Department.Add(model);
                try
                {
                    db.SaveChanges();
                    DBCaches2.ClearCache("cache_depts");
                }
                catch
                {
                    ViewBag.msg = "部门添加失败，请重试。";
                }
            }
            ViewData["DeptList"] = DBCaches2.getDeptCache();
            return View(info);
        }
        [wxAuthorize(Roles = "系统管理员")]
        public JsonResult DeleteDept(string pid)
        {
            int id = PageValidate.FilterParam(pid);
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                goto next;
            }
            Dic_Department model = db.Dic_Department.Find(id);
            if (model == null)
            {
                json.msg_text = "没有找到该部门/科室，该部门/科室可能已被删除。";
                json.msg_code = "noThis";
                goto next;
            }
            db.Dic_Department.Remove(model);
            try
            {
                db.SaveChanges();
                DBCaches2.ClearCache("cache_depts");
            }
            catch
            {
                json.msg_text = "删除失败，请重新操作。";
                json.msg_code = "recyErr";
                goto next;
            }
            json.state = 1;
            json.msg_code = "success";
            json.msg_text = "删除成功！";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 角色设置
        [wxAuthorize(Roles = "系统管理员")]
        public ActionResult Role()
        {
            ViewData["RoleList"] = DBCaches<Dic_Role>.getCache("cache_role"); ;
            return View(new Dic_Role());
        }
        [HttpPost, wxAuthorize(Roles = "系统管理员")]
        [ValidateAntiForgeryToken]
        public ActionResult Role(Dic_Role model)
        {
            model.role_name = PageValidate.InputText(model.role_name, 50);
            if (db.Dic_Role.Where(x => x.role_name == model.role_name).Count() > 0) ViewBag.msg = "角色名称已存在";
            else
            {
                db.Dic_Role.Add(model);
                try
                {
                    db.SaveChanges();
                    DBCaches<Dic_Role>.ClearCache("cache_role");
                }
                catch
                {
                    ViewBag.msg = "角色添加失败，请重试。";
                }
            }
            ViewData["RoleList"] = DBCaches<Dic_Role>.getCache("cache_role");// db.Dic_Post.ToList();
            return View(model);
        }
        [wxAuthorize(Roles = "系统管理员")]
        public JsonResult DeleteRole(string rid)
        {
            int id = PageValidate.FilterParam(rid);
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                goto next;
            }
            Dic_Role model = db.Dic_Role.Find(id);
            if (model == null)
            {
                json.msg_text = "没有找到该角色，该角色可能已被删除。";
                json.msg_code = "noThis";
                goto next;
            }
            db.Dic_Role.Remove(model);
            try
            {
                db.SaveChanges();
                DBCaches<Dic_Role>.ClearCache("cache_role");
            }
            catch
            {
                json.msg_text = "删除失败，请重新操作。";
                json.msg_code = "recyErr";
                goto next;
            }
            json.state = 1;
            json.msg_code = "success";
            json.msg_text = "删除成功！";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion
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
