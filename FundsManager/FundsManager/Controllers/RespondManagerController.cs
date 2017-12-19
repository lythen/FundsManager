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

        // GET: RespondManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RespondManager/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pr_id,pr_apply_number,pr_user_id,pr_number,pr_time,pr_content,pr_state")] Process_Respond process_Respond)
        {
            if (ModelState.IsValid)
            {
                db.Process_Respond.Add(process_Respond);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(process_Respond);
        }

        // GET: RespondManager/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: RespondManager/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pr_id,pr_apply_number,pr_user_id,pr_number,pr_time,pr_content,pr_state")] Process_Respond process_Respond)
        {
            if (ModelState.IsValid)
            {
                db.Entry(process_Respond).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(process_Respond);
        }

        // GET: RespondManager/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: RespondManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Process_Respond process_Respond = db.Process_Respond.Find(id);
            db.Process_Respond.Remove(process_Respond);
            db.SaveChanges();
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
