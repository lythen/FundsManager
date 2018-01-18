using System.Data;
using System.Linq;
using System.Web.Mvc;
using FundsManager.DAL;
using FundsManager.Models;
using System.Web.Security;
using System;
using System.Web;
using FundsManager.ViewModels;
using FundsManager.Common;
using System.Data.Entity;
using FundsManager.Common.DEncrypt;

namespace FundsManager.Controllers
{
    public class LoginController : Controller
    {
        private FundsContext db = new FundsContext();
        // GET: Login
        public ActionResult Index()
        {
            LoginModel model = new LoginModel();
            if (Request.Cookies["name"] != null)
            {
                model.userName = Server.UrlDecode(Request.Cookies["name"].Value);
                model.isRemember = true;
            }
            return View(model);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "userName,password,checkCode,isRemember")]LoginModel model)
        {
            if (Session["ErrorPsw"] == null) Session["ErrorPsw"] = 0;
            int errTimes = (int)Session["ErrorPsw"];
            //if (errTimes >= 5)
            //{
            //    ViewBag.msg = "失败次数过多，请1小时后再尝试。";
            //    return View(model);
            //}
            //List<SelectOption> options = DropDownList.SysRolesSelect();
            //ViewBag.ddlRoles = DropDownList.SetDropDownList(options);
            if (Session["checkCode"] == null)
            {
                ViewBag.msg = "验证码已过期，请点击验证码刷新后重新输入密码码。";
                errTimes++;
                Session["ErrorPsw"] = errTimes;
                return View(model);
            }
            if (model.checkCode.ToUpper() != Session["checkCode"].ToString())
            {
                ViewBag.msg = "验证码不正确。";
                
                return View(model);
            }

            //验证帐号密码
            var user = (from p in db.User_Info
                        join uvr in db.User_vs_Role
                        on p.user_id equals uvr.uvr_user_id
                        where p.user_name == model.userName
                        select p
                        ).FirstOrDefault();
            if (user == null)
            {
                ViewBag.msg = "用户不存在。";
                return View(model);
            }
            string password = AESEncrypt.Encrypt(PasswordUnit.getPassword(PageValidate.InputText(model.password,40).ToUpper(), user.user_salt));
            if (password != user.user_password)
            {
                ViewBag.msg = "用户密码不正确，请重新输入。";
                return View(model);
            }
            if (user.user_state == 0)
            {
                ViewBag.msg = "您的帐号被锁定,暂时无法登陆。";
                return View(model);
            }
            if (user.user_state != 1)
            {
                ViewBag.msg = "您的帐号异常,暂时无法登陆。";
                return View(model);
            }
            //验证权限
            var role = (from uvr in db.User_vs_Role
                        join r in db.Dic_Role
                        on uvr.uvr_role_id equals r.role_id
                        where uvr.uvr_user_id == user.user_id
                        select new LoginRole
                        {
                            roleId = r.role_id,
                            roleName = r.role_name
                        }).FirstOrDefault();
            if (role == null || role.roleId == 0 || role.roleId > 5)
            {
                ViewBag.msg = "没有权限登陆所选角色。";
                return View(model);
            }
            //功能权限
            var controlroles = (from r in db.Dic_Role
                                join rvc in db.Role_vs_Controller
                                on r.role_id equals rvc.rvc_role_id
                                where r.role_id == role.roleId
                                select rvc.rvc_controller
                       ).ToArray();
            string ip = IpHelper.GetIP();
            string loginDev = string.Format("{0}-{1}-{2}-{3}-{4}"
                , Request.Browser.Id
                , Request.Browser.MobileDeviceManufacturer
                , Request.Browser.MobileDeviceModel
                , Request.Browser.Platform
                , Request.Browser.Type
                );
            Sys_Log log = new Sys_Log
            {
                log_content = "登陆",
                log_time = DateTime.Now,
                log_user_id = user.user_id,
                log_ip = ip,
                log_target = user.user_id.ToString(),
                log_type = 1,
                log_device = loginDev
            };
            user.user_login_times++;
            db.Sys_Log.Add(log);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            user.ToDecrypt();
            user.DeletePassword();

            Session["LoginRole"] = role;
            Session["ControlRoles"] = controlroles;
            Session["UserInfo"] = user;
            DataCache.SetCache("user-roles-" + user.user_id, role);
            HttpCookie cookie;
            if (model.isRemember)
            {
                cookie = new HttpCookie("name", Server.UrlEncode(model.userName));
                cookie.Expires = DateTime.Now.AddHours(1);
                Response.AppendCookie(cookie);
            }
            else if (Request.Cookies["name"] != null) Response.Cookies.Remove("name");

            cookie = new HttpCookie("realname", Server.UrlEncode(user.real_name));
            cookie.Expires = DateTime.Now.AddHours(1);
            Response.AppendCookie(cookie);

            FormsAuthentication.SetAuthCookie(user.user_id.ToString(), true);
            return RedirectToRoute(new { controller = "Home", action = "Index" });
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index");
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
