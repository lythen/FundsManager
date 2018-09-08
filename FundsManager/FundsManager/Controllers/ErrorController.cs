using System.Web.Mvc;
using Lythen.DAL;

namespace Lythen.Controllers
{
    public class ErrorController : Controller
    {
        private LythenContext db = new LythenContext();

        // GET: Error
        public ActionResult Index(string err)
        {
            if (err == "没有权限!")
            {
                if (Session["UserInfo"] == null)
                    return RedirectToRoute(new { controller = "Login", action = "Logout" });
            }
            ViewBag.msg = err;
            return View();
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
