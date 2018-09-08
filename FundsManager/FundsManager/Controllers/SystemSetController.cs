using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Lythen.DAL;
using Lythen.Models;
using Lythen.ViewModels;
using Lythen.Common;
namespace Lythen.Controllers
{
    public class SystemSetController : Controller
    {
        private FundsContext db = new FundsContext();

        #region 网站设置
        public ActionResult SiteInfo()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限当前内容。" });

            ViewModels.SiteInfo info = Lythen.Controllers.SiteInfo.getSiteInfo();
            return View(info);
        }
        public ActionResult SiteSet()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限执行当前操作。" });

            ViewModels.SiteInfo info = Lythen.Controllers.SiteInfo.getSiteInfo();
            return View(info);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SiteSet([Bind(Include = "name,company,introduce,companyAddress,companyPhone,companyEmail,managerName,managerPhone,managerEmail")]ViewModels.SiteInfo info)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限执行当前操作。" });

            Sys_SiteInfo model = db.Sys_SiteInfo.FirstOrDefault();
            if (model != null)
            {
                db.Sys_SiteInfo.Remove(model);
                db.SaveChanges();
            }
            model = new Sys_SiteInfo();
            info.toDBModel(model);
            db.Sys_SiteInfo.Add(model);

            try
            {
                db.SaveChanges();
                DBCaches<Sys_SiteInfo>.ClearCache("site-name");
                DBCaches<Sys_SiteInfo>.ClearCache("site-info");
            }
            catch (Exception ex)
            {
                @ViewBag.msg = "修改失败。";
            }
            SysLog.WriteLog(user, "修改网站信息", IpHelper.GetIP(), "", 5, "", db);
            @ViewBag.msg = "修改成功。";
            return View(info);
        }
        #endregion
        #region 模块设置
        public ActionResult ContrlModule()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限执行当前操作。" });

            List<ModuleInfo> models = DBCaches2.getModuleInfo();
            foreach (ModuleInfo model in models)
            {
                int[] roles = (from rvc in db.Role_vs_Controller
                               where rvc.rvc_controller == model.name
                               select rvc.rvc_role_id
                               ).ToArray();
                RoleInfo[] rvcs = DBCaches2.getRoleInfo();
                foreach (RoleInfo item in rvcs)
                {
                    if (roles.Contains(item.id)) item.hasrole = true;
                    else item.hasrole = false;
                }
                model.roles = rvcs;
            }
            return View(models);
        }
        public JsonResult ContrlModule(EditModules models)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                goto next;
            }
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "NoPower";
                goto next;
            }
            if (ModelState.IsValid)
            {
                string ctrl_name;
                foreach (ModuleInfo info in models.modules)
                {
                    ctrl_name = info.name;
                    var no1 = db.Role_vs_Controller.Where(x => x.rvc_role_id != 1 && x.rvc_controller == ctrl_name);
                    if (no1.Count() > 0)
                    {
                        db.Role_vs_Controller.RemoveRange(no1);
                        db.SaveChanges();
                    }
                    if (info.roles != null && info.roles.Length > 0)
                    {
                        foreach (RoleInfo rinfo in info.roles)
                        {
                            Role_vs_Controller rvc = new Role_vs_Controller();
                            rvc.rvc_role_id = rinfo.id;
                            rvc.rvc_controller = ctrl_name;
                            if (db.Role_vs_Controller.Find(rvc.rvc_role_id, rvc.rvc_controller) == null) db.Role_vs_Controller.Add(rvc);
                        }
                    }
                }
                db.SaveChanges();
                SysLog.WriteLog(user, "修改系统模块", IpHelper.GetIP(), "", 5, "", db);
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
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 职务设置
        public ActionResult Post()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限执行当前操作。" });

            ViewData["PostList"] = DBCaches<Dic_Post>.getCache("cache_post"); ;
            return View(new Dic_Post());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Post(Dic_Post model)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限执行当前操作。" });

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
                SysLog.WriteLog(user, string.Format("添加职务[{0}]",model.post_name), IpHelper.GetIP(), "", 5, "", db);
            }
            ViewData["PostList"] = DBCaches<Dic_Post>.getCache("cache_post");// db.Dic_Post.ToList();
            return View(model);
        }
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
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "NoPower";
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
            SysLog.WriteLog(user, string.Format("删除职务[{0}]", model.post_name), IpHelper.GetIP(), "", 5, "", db);
            json.state = 1;
            json.msg_code = "success";
            json.msg_text = "删除成功！";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdatePost(Dic_Post post)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                goto next;
            }
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "NoPower";
                goto next;
            }
            if (post.post_id == 0)
            {
                json.msg_text = "获取部门/科室的ID出错。";
                json.msg_code = "IDError";
                goto next;
            }

            var same = db.Dic_Post.Where(x => x.post_name == post.post_name && x.post_id != post.post_id);
            if (same.Count() > 0)
            {
                json.msg_text = "该名称已存在。";
                json.msg_code = "NameExists";
                goto next;
            }
            db.Entry(post).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                DBCaches<Dic_Post>.ClearCache("cache_post");
            }
            catch
            {
                json.msg_text = "更新，请重新操作。";
                json.msg_code = "UpdateErr";
                goto next;
            }
            SysLog.WriteLog(user, string.Format("更新职务[{0}]", post.post_name), IpHelper.GetIP(), "", 5, "", db);
            json.state = 1;
            json.msg_code = "success";
            json.msg_text = "更新成功！";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 科室部门管理
        public ActionResult Department()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限执行当前操作。" });

            List<SelectOption> options = DropDownList.getDepartment();
            ViewBag.Dept = DropDownList.SetDropDownList(options);
            ViewData["DeptList"] = DBCaches2.getDeptCache();
            return View(new DepartMentModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Department(DepartMentModel info)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限执行当前操作。" });

            List<SelectOption> options = DropDownList.getDepartment();
            ViewBag.Dept = DropDownList.SetDropDownList(options);
            Dic_Department model = new Dic_Department();
            model.dept_name = PageValidate.InputText(info.deptName, 50);
            if (db.Dic_Department.Where(x => x.dept_name == model.dept_name && x.dept_parent_id == info.parentId).Count() > 0) ViewBag.msg = "名称已存在";
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
            SysLog.WriteLog(user, string.Format("添加部门[{0}]", model.dept_name), IpHelper.GetIP(), "", 5, "", db);
            ViewData["DeptList"] = DBCaches2.getDeptCache();
            return View(info);
        }
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
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "NoPower";
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
            SysLog.WriteLog(user, string.Format("删除部门[{0}]", model.dept_name), IpHelper.GetIP(), "", 5, "", db);
            json.state = 1;
            json.msg_code = "success";
            json.msg_text = "删除成功！";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateDept(DepartMentModel dept)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                goto next;
            }
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "NoPower";
                goto next;
            }
            if (dept.deptId == null || dept.deptId == 0)
            {
                json.msg_text = "获取部门/科室的ID出错。";
                json.msg_code = "IDError";
                goto next;
            }
            int id = (int)dept.deptId;
            Dic_Department model = db.Dic_Department.Find(id);
            if (model == null)
            {
                json.msg_text = "没有找到该部门/科室，该部门/科室可能已被删除。";
                json.msg_code = "noThis";
                goto next;
            }
            var same = db.Dic_Department.Where(x => x.dept_name == dept.deptName && x.dept_id != dept.deptId);
            if (same.Count() > 0)
            {
                json.msg_text = "该名称已存在。";
                json.msg_code = "NameExists";
                goto next;
            }
            model.dept_name = dept.deptName;
            model.dept_parent_id = dept.parentId;

            db.Entry(model).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                DBCaches2.ClearCache("cache_depts");
            }
            catch
            {
                json.msg_text = "更新，请重新操作。";
                json.msg_code = "UpdateErr";
                goto next;
            }
            SysLog.WriteLog(user, string.Format("更新部门[{0}]", model.dept_name), IpHelper.GetIP(), "", 5, "", db);
            json.state = 1;
            json.msg_code = "success";
            json.msg_text = "更新成功！";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 角色设置
        public ActionResult RoleAuthSet()
        {
            List<SelectOption> options = DropDownList.RoleSelect("系统管理员");
            ViewData["Roles"] = DropDownList.SetDropDownList(options);
            List<AuthInfo> auths = DropDownList.AuthoritySelect();
            ViewData["Auths"] = auths;
            return View();
        }
        public JsonResult GetRoleAuth(int roleId)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                goto next;
            }
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "NoPower";
                goto next;
            }
            if (roleId == 0)
            {
                json.msg_text = "获取角色出错。";
                json.msg_code = "IDError";
                goto next;
            }
            var rvas = from rva in db.Role_vs_Authority
                       where rva.rva_role_id == roleId
                       select new ViewRoleAuthority
                       {
                           authId = rva.rva_auth_id,
                           roleId = rva.rva_role_id
                       };
            if (rvas.Count() == 0)
            {
                json.state = 0;
                json.msg_code = "noData";
                json.msg_text = "没有数据。";
            }
            else
            {
                json.state = 1;
                json.data = rvas.ToList();
            }
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SetRoleAuth(List<ViewRoleAuthority> auths)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                goto next;
            }
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "NoPower";
                goto next;
            }
            if (auths == null || auths.Count() == 0)
            {
                json.msg_text = "没有接收任何数据。";
                json.msg_code = "NoReceive";
                goto next;
            }
            bool firstIn = true;
            foreach (ViewRoleAuthority item in auths)
            {
                if (firstIn)
                {
                    db.Role_vs_Authority.RemoveRange(db.Role_vs_Authority.Where(x => x.rva_role_id == item.roleId));
                    firstIn = false;
                }
                Role_vs_Authority rva = new Role_vs_Authority()
                {
                    rva_auth_id = item.authId,
                    rva_role_id = item.roleId
                };
                db.Role_vs_Authority.Add(rva);
            }
            try
            {
                db.SaveChanges();
                json.state = 1;
                json.msg_text = "角色的权限修改成功。";
                json.msg_code = "success";
            }
            catch (Exception ex)
            {
                json.msg_text = "角色权限修改出错。";
                json.msg_code = "error";
                Common.ErrorUnit.WriteErrorLog(ex.ToString(), this.GetType().ToString());
            }
            SysLog.WriteLog(user, "重置角色的权限", IpHelper.GetIP(), "", 5, "", db);
            //重设置角色权限后，必需清除缓存
            DataCache.RemoveCacheBySearch("user_vs_roles");
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Role()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限当前内容。" });
            ViewData["RoleList"] = DBCaches<Dic_Role>.getCache("cache_role"); ;
            return View(new Dic_Role());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Role(Dic_Role model)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限当前内容。" });

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

            SysLog.WriteLog(user, string.Format("添加角色[{0}]", model.role_name), IpHelper.GetIP(), "", 5, "", db);
            ViewData["RoleList"] = DBCaches<Dic_Role>.getCache("cache_role");// db.Dic_Post.ToList();
            return View(model);
        }
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
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "NoPower";
                goto next;
            }
            if (id == 1)
            {
                json.msg_text = "该角色不允许删除。";
                json.msg_code = "CanNotDel";
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
            SysLog.WriteLog(user, string.Format("删除角色[{0}]", model.role_name), IpHelper.GetIP(), "", 5, "", db);
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRole(Dic_Role model)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                goto next;
            }
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "NoPower";
                goto next;
            }
            if (model.role_id == 0)
            {
                json.msg_text = "获取角色的ID出错。";
                json.msg_code = "IDError";
                goto next;
            }
            if (model.role_id == 1)
            {
                json.msg_text = "该角色不允许修改。";
                json.msg_code = "CanNotUpdate";
                goto next;
            }
            var same = db.Dic_Post.Where(x => x.post_name == model.role_name && x.post_id != model.role_id);
            if (same.Count() > 0)
            {
                json.msg_text = "该名称已存在。";
                json.msg_code = "NameExists";
                goto next;
            }
            db.Entry(model).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                DBCaches<Dic_Post>.ClearCache("cache_post");
            }
            catch
            {
                json.msg_text = "更新，请重新操作。";
                json.msg_code = "UpdateErr";
                goto next;
            }
            json.state = 1;
            json.msg_code = "success";
            json.msg_text = "更新成功！";
            SysLog.WriteLog(user, string.Format("更新角色[{0}]名称", model.role_name), IpHelper.GetIP(), "", 5, "", db);
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 日志查询
        public ActionResult Logs(StatisticsSearch search)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            List<SelectOption> options = DropDownList.UserSelect(user);
            ViewData["Users"] = DropDownList.SetDropDownList(options);
            var query = from log in db.Sys_Log
                        join t in db.Dic_Log_Type on log.log_type equals t.dlt_log_id into T
                        from t1 in T.DefaultIfEmpty()
                        join u in db.User_Info on log.log_user_id equals u.user_id into U
                        from u1 in U.DefaultIfEmpty()
                        select new ViewLogsModel
                        {
                            uid = log.log_user_id,
                            user = u1.real_name,
                            info = log.log_content,
                            ip = log.log_ip,
                            id = log.log_id,
                            time = log.log_time,
                            device = log.log_device,
                            target = log.log_target,
                            targetStr = log.log_target,
                            type = log.log_type,
                            typeStr = t1.dlt_log_name
                        };
            if (!RoleCheck.CheckHasAuthority(user, db, "经费管理"))
                query = query.Where(x => x.uid == user);
            if (search.beginDate != null)
            {
                search.beginDate = DateTime.Parse(((DateTime)search.beginDate).ToString("yyyy-MM-dd 00:00:00.000"));
                query = query.Where(x => x.time >= search.beginDate);
            }
            if (search.endDate != null)
            {
                search.endDate = DateTime.Parse(((DateTime)search.endDate).ToString("yyyy-MM-dd 23:59:59.999"));
                query = query.Where(x => x.time <= search.endDate);
            }
            if (search.userId != null && search.userId != 0)
                query = query.Where(x => x.uid == search.userId);
            string keyword= PageValidate.InputText(search.KeyWord, 100);
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.info.Contains(keyword));
            //统计总条目
            search.Amount = query.Count();
            //分页
            query = query.OrderByDescending(x => x.time).Skip(search.PageSize * (search.PageIndex - 1)).Take(search.PageSize);
            var list = query.ToList();
            foreach (var item in list)
            {
                if (item.type == 2)
                {
                    int uid = PageValidate.FilterParam(item.target);
                    var t = (from u in db.User_Info
                             where u.user_id == item.uid
                             select u.real_name).FirstOrDefault();
                    if(!string.IsNullOrEmpty(t))
                    item.targetStr = Common.DEncrypt.AESEncrypt.Decrypt(t);
                }
                item.user = Common.DEncrypt.AESEncrypt.Decrypt(item.user);
            }
            ViewData["Logs"] = list;
            
            return View(search);
        }
        #endregion

        public JsonResult DeleteAllCache()
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "系统管理"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "NoPower";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            DataCache.RemoveAllCache();
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
