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

namespace FundsManager.Controllers
{
    public class RespondManagerController : Controller
    {
        private FundsContext db = new FundsContext();

        // GET: RespondManager
        public ActionResult Index()
        {
            return View(db.Process_Respond.ToList());
        }

        // GET: RespondManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Process_Respond process_Respond = db.Process_Respond.Find(id);
            if (process_Respond == null)
            {
                return HttpNotFound();
            }
            return View(process_Respond);
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
