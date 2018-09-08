using Lythen.DAL;
using Lythen.Models;
using Lythen.ViewModels;
using System.Web.Mvc;
using System.Linq;
using Lythen.Common.DEncrypt;
namespace Lythen.Controllers
{
    public class HomeController : Controller
    {
        private LythenContext db = new LythenContext();
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
                                    name = u.real_name,
                                    times = u.user_login_times
                                }).FirstOrDefault();
                var loginInfos = (from l in db.Sys_Log
                                  where l.log_user_id == id
                                  select l
                                 );
                if (loginInfos.Count() <= 1)
                {
                    userInfo.lastIp = "无";
                    userInfo.lastTime = "无";

                }
                else
                {
                    var loginInfo = loginInfos.OrderByDescending(x=>x.log_time).Skip(1).FirstOrDefault();
                    userInfo.lastIp = loginInfo.log_ip;
                    userInfo.lastTime = loginInfo.log_time.ToString("yyyy年MM月dd日 HH时mm分");
                }
                userInfo.roleName = role.roleName;
                userInfo.name = AESEncrypt.Decrypt(userInfo.name);
                //如果是有批复权限的，显示待批复列表
                return View(userInfo);
            }
        }
    }
}