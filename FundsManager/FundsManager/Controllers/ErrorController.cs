﻿using System.Web.Mvc;
using FundsManager.DAL;

namespace WeChatForTraining.Controllers
{
    public class ErrorController : Controller
    {
        private FundsContext db = new FundsContext();

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
