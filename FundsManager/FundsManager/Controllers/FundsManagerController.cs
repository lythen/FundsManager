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
    public class FundsManagerController : Controller
    {
        private FundsContext db = new FundsContext();

        // GET: FundsManager
        public ActionResult Index()
        {
            return View(db.Funds.ToList());
        }

        // GET: FundsManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funds funds = db.Funds.Find(id);
            if (funds == null)
            {
                return HttpNotFound();
            }
            return View(funds);
        }

        // GET: FundsManager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FundsManager/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "f_id,f_name,f_source,f_amount,f_balance,f_manager,f_info")] Funds funds)
        {
            if (ModelState.IsValid)
            {
                db.Funds.Add(funds);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(funds);
        }

        // GET: FundsManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funds funds = db.Funds.Find(id);
            if (funds == null)
            {
                return HttpNotFound();
            }
            return View(funds);
        }

        // POST: FundsManager/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "f_id,f_name,f_source,f_amount,f_balance,f_manager,f_info")] Funds funds)
        {
            if (ModelState.IsValid)
            {
                db.Entry(funds).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(funds);
        }

        // GET: FundsManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funds funds = db.Funds.Find(id);
            if (funds == null)
            {
                return HttpNotFound();
            }
            return View(funds);
        }

        // POST: FundsManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Funds funds = db.Funds.Find(id);
            db.Funds.Remove(funds);
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
