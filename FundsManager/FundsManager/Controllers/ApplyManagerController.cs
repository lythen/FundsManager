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
    public class ApplyManagerController : Controller
    {
        private FundsContext db = new FundsContext();

        // GET: ApplyManager
        public ActionResult Index()
        {
            return View(db.Funds_Apply.ToList());
        }

        // GET: ApplyManager/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funds_Apply funds_Apply = db.Funds_Apply.Find(id);
            if (funds_Apply == null)
            {
                return HttpNotFound();
            }
            return View(funds_Apply);
        }

        // GET: ApplyManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApplyManager/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "apply_number,apply_user_id,apply_time,apply_funds_id,apply_for,apply_amount,apply_state")] Funds_Apply funds_Apply)
        {
            if (ModelState.IsValid)
            {
                db.Funds_Apply.Add(funds_Apply);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(funds_Apply);
        }

        // GET: ApplyManager/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funds_Apply funds_Apply = db.Funds_Apply.Find(id);
            if (funds_Apply == null)
            {
                return HttpNotFound();
            }
            return View(funds_Apply);
        }

        // POST: ApplyManager/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "apply_number,apply_user_id,apply_time,apply_funds_id,apply_for,apply_amount,apply_state")] Funds_Apply funds_Apply)
        {
            if (ModelState.IsValid)
            {
                db.Entry(funds_Apply).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(funds_Apply);
        }

        // GET: ApplyManager/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funds_Apply funds_Apply = db.Funds_Apply.Find(id);
            if (funds_Apply == null)
            {
                return HttpNotFound();
            }
            return View(funds_Apply);
        }

        // POST: ApplyManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Funds_Apply funds_Apply = db.Funds_Apply.Find(id);
            db.Funds_Apply.Remove(funds_Apply);
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
