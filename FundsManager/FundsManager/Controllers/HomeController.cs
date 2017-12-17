using FundsManager.DAL;
using FundsManager.Models;
using FundsManager.ViewModels;
using System.Web.Mvc;
using System.Linq;
namespace FundsManager.Controllers
{
    public class HomeController : Controller
    {
        private FundsContext db = new FundsContext();
        // GET: Home
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "Index" });
            else
            {
                if (Session["UserInfo"] == null || Session["LoginRole"] == null || Session["ControlRoles"] == null) return RedirectToRoute(new { controller = "Login", action = "Index" });
                User_Info user = (User_Info)Session["UserInfo"];
                LoginRole role = (LoginRole)Session["LoginRole"];
                int id = Common.PageValidate.FilterParam(User.Identity.Name);
                var userInfo = (from u in db.User_Info
                                where u.user_id == id
                                select new UserModel
                                {
                                    name = u.user_name,
                                    times = u.user_login_times
                                }).FirstOrDefault();
                var loginInfo = (from l in db.Sys_Log
                                 where l.log_user_id == id
                                 orderby l.log_time ascending
                                 select l
                                 ).FirstOrDefault();
                if (loginInfo != null)
                {
                    userInfo.lastIp = loginInfo.log_ip;
                    userInfo.lastTime = loginInfo.log_time.ToString("yyyy年MM月dd日 HH时mm分");
                    userInfo.roleName = role.roleName;
                }
                //如果是有批复权限的，显示待批复列表
                return View(userInfo);
            }
        }
    }
}