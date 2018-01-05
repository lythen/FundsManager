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
    public class FundsManagerController : Controller
    {
        private FundsContext db = new FundsContext();

        // GET: FundsManager
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            //管理的经费
            var mfunds = (from funds in db.Funds
                          where funds.f_manager == user
                          select new mFundsListModel
                          {
                              amount = funds.f_amount,
                              balance = funds.f_balance,
                              expireDate = funds.f_expireDate,
                              id = funds.f_id,
                              name = funds.f_name,
                              strState = funds.f_state == 0 ? "未启用" : (funds.f_state == 1 ? "正常" : "锁定"),
                              userCount = (from fac in db.Funds_Apply_Child
                                           join fa in db.Funds_Apply
                                           on fac.c_apply_number equals fa.apply_number
                                           where fac.c_funds_id == funds.f_id && fa.apply_state == 1
                                           select fac
                                          ).Count(),
                              applyamount =  (
                                from fac in db.Funds_Apply_Child
                                join fa in db.Funds_Apply
                                on fac.c_apply_number equals fa.apply_number
                                where fac.c_funds_id == funds.f_id && fa.apply_state == 1
                                select fac.c_amount
                                ).DefaultIfEmpty(0).Sum()
                          }
                          ).ToList();
            //使用的经费
            var ufunds = (from funds in db.Funds
                          join fac in db.Funds_Apply_Child
                          on funds.f_id equals fac.c_funds_id
                          join apply in db.Funds_Apply
                          on fac.c_apply_number equals apply.apply_number
                          join u in db.User_Info
                          on funds.f_manager equals u.user_id into T1
                          from t1 in T1.DefaultIfEmpty()
                          where apply.apply_user_id == user && apply.apply_state == 1
                          select new uFundsListModel
                          {
                              amount = fac.c_amount,
                              expireDate = funds.f_expireDate,
                              managerName = t1.user_name,
                              name = funds.f_name
                          }
                          ).ToList();
            FundsListView list = new FundsListView();
            list.managerFunds = mfunds;
            list.useFunds = ufunds;
            return View(list);
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
            SetSelect();
            return View(new FundsModel());
        }
        void SetSelect()
        {
            List<SelectOption> options = DropDownList.UserStateSelect();
            ViewBag.State = DropDownList.SetDropDownList(options);
            options = DropDownList.FundsManagerSelect();
            ViewBag.Manager = DropDownList.SetDropDownList(options);
        }
        // POST: FundsManager/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,expireDate,source,amount,balance,manager,info,state")] FundsModel funds)
        {
            if (ModelState.IsValid)
            {
                if (db.Funds.Where(x => x.f_name == funds.name.ToString()).Count() > 0)
                {
                    ViewBag.msg = "该名称已被使用";
                    return View(funds);
                }
                Funds model = new Funds();
                funds.toDBModel(model);
                db.Funds.Add(model);
                db.SaveChanges();
                ViewBag.msg = "经费添加成功。";
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
