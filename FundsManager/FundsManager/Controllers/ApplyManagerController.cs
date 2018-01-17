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
using FundsManager.ViewModels;

namespace FundsManager.Controllers
{
    public class ApplyManagerController : Controller
    {
        private FundsContext db = new FundsContext();

        // GET: ApplyManager
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            var waitList = (from apply in db.Funds_Apply
                            join das in db.Dic_Apply_State
                            on apply.apply_state equals das.das_state_id
                            where apply.apply_user_id == user && apply.apply_state!=1
                            select new ApplyListModel
                            {
                                amount = apply.apply_amount,
                                number = apply.apply_number,
                                state = apply.apply_state,
                                time = apply.apply_time,
                                strState= das.das_state_name
                            }
                            ).ToList();
            return View(waitList);
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
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            SetSelect();
            ApplyEditModel model = new ApplyEditModel();
            List<ApplyChildModel> list = new List<ApplyChildModel>();
            list.Add(new ApplyChildModel());
            model.capply = list;
            return View(model);
        }

        // POST: ApplyManager/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApplyEditModel funds_Apply)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            SetSelect();
            if (ModelState.IsValid)
            {
                //db.Funds_Apply.Add(funds_Apply);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(funds_Apply);
        }
        void SetSelect()
        {
            List<SelectOption> options = DropDownList.FundsSelect();
            ViewBag.Funds = DropDownList.SetDropDownList(options);
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
        public ActionResult MyFunds()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            var waitList = (from apply in db.Funds_Apply
                            join das in db.Dic_Apply_State
                            on apply.apply_state equals das.das_state_id
                            where apply.apply_user_id == user && apply.apply_state==1
                            select new ApplyListModel
                            {
                                amount = apply.apply_amount,
                                number = apply.apply_number,
                                state = apply.apply_state,
                                time = apply.apply_time,
                                strState = das.das_state_name
                            }
                            ).ToList();
            return View(waitList);
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
