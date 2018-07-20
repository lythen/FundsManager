using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FundsManager.DAL;
using FundsManager.Models;
using FundsManager.ViewModels;
using System.Data.Entity.Validation;
using System.Text;
using FundsManager.Common;

namespace FundsManager.Controllers
{
    public class FundsManagerController : Controller
    {
        private FundsContext db = new FundsContext();

        // GET: FundsManager
        public ActionResult Index(BillsSearchModel info)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            bool isAdmin = RoleCheck.CheckHasAuthority(user,db, "经费管理", "添加经费");
            if (!isAdmin) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限。" });
            ApplyManager dal = new ApplyManager(db);
            if (info.userId != 0)
            {
                if (!RoleCheck.CheckHasAuthority(user, db, "经费管理")) info.userId = user;
            }
            info.PageSize = 0;
            //管理的经费
            var mfunds = from funds in db.Funds
                          where funds.f_manager == (isAdmin? funds.f_manager: user)
                          select new mFundsListModel
                          {
                              manager=funds.f_manager,
                              code=funds.f_code,
                              amount = funds.f_amount,
                              balance = funds.f_balance,
                              id = funds.f_id,
                              name = funds.f_name,
                              strState = funds.f_state == 0 ? "未启用" : (funds.f_state == 1 ? "正常" : "锁定"),
                              userCount = (from bill in db.Reimbursement
                                           where bill.r_funds_id == funds.f_id && bill.r_bill_state == 1
                                           select bill
                                          ).Count(),
                              applyamount = (
                                from bill in db.Reimbursement
                                where bill.r_funds_id == funds.f_id && bill.r_bill_state == 1
                                select bill.r_fact_amount
                                ).DefaultIfEmpty(0).Sum()
                          };
            if (info.userId > 0) mfunds = mfunds.Where(x => x.manager == user);
            ViewData["Funds"] = mfunds.ToList();
            List<SelectOption> options = DropDownList.FundsManagerSelect((int)info.userId);
            ViewData["ViewUsers"] = DropDownList.SetDropDownList(options);
            return View(info);
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
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "添加经费", "经费管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限。" });
            SetSelect();
            return View(new FundsModel());
        }
        void SetSelect()
        {
            List<SelectOption> options = DropDownList.UserStateSelect();
            ViewBag.State = DropDownList.SetDropDownList(options);
            //options = DropDownList.FundsManagerSelect();
            //ViewBag.Manager = DropDownList.SetDropDownList(options);
            options = DropDownList.ProcessSelect();
            ViewBag.Process = DropDownList.SetDropDownList(options);

        }
        // POST: FundsManager/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "code,name,source,amount,balance,processId,info,state")] FundsModel funds)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            if(!RoleCheck.CheckHasAuthority(user, db, "添加经费", "经费管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限。" });
            SetSelect();
            if (ModelState.IsValid)
            {
                if (db.Funds.Where(x => x.f_code == funds.code).Count() > 0)
                {
                    ViewBag.msg = "该代码已被使用";
                    return View(funds);
                }
                if (db.Funds.Where(x => x.f_name == funds.name).Count() > 0)
                {
                    ViewBag.msg = "该名称已被使用";
                    return View(funds);
                }
                //if (funds.processId == null || funds.processId == 0)
                //{
                //    ViewBag.msg = "未选择批复流程。";
                //    return View(funds);
                //}
                if (funds.amount == 0)
                {
                    ViewBag.msg = "请设置经费总额。";
                    return View(funds);
                }
                Funds model = new Funds();
                funds.toDBModel(model);
                model.f_manager = user;
                db.Funds.Add(model);
                db.SaveChanges();
                ViewBag.msg = "经费添加成功。";
            }

            return View(funds);
        }

        // GET: FundsManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "添加经费", "经费管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限。" });
            SetSelect();
            FundsModel funds = (from f in db.Funds
                                where f.f_id == (int)id
                                select new FundsModel
                                {
                                    amount = f.f_amount,
                                    id = f.f_id,
                                    balance = f.f_balance,
                                    info = f.f_info,
                                    manager = f.f_manager,
                                    name = f.f_name,
                                    source = f.f_source,
                                    state = f.f_state,
                                    code=f.f_code
                                }).FirstOrDefault();
            if (funds == null)
            {
                return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有找到该经费。" });
            }
            if (user != funds.manager&& !RoleCheck.CheckHasAuthority(user, db, "经费管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有对该经费的管理权限。" });
            return View(funds);
        }

        // POST: FundsManager/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,code,name,expireDate,source,amount,balance,info,state")] FundsModel funds)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "添加经费", "经费管理")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限。" });
            SetSelect();
            if (ModelState.IsValid)
            {
                Funds model = db.Funds.Find(funds.id);
                if (funds == null)
                {
                    ViewBag.msg = "没有找到该经费。";
                    return View(funds);
                }
                if (user != model.f_manager && !RoleCheck.CheckHasAuthority(user, db, "经费管理"))
                {
                    ViewBag.msg = "您不是该经费的管理员，没有更改权限。";
                    return View(funds);
                }
                if (model.f_name != funds.name)
                    if (db.Funds.Where(x => x.f_name == funds.name && x.f_id != funds.id).Count() > 0)
                    {
                        ViewBag.msg = "该名称已被使用";
                        return View(funds);
                    }
                if (model.f_code != funds.code)
                {
                    if (db.Funds.Where(x => x.f_code == funds.code && x.f_id != funds.id).Count() > 0)
                    {
                        ViewBag.msg = "该代码已被使用";
                        return View(funds);
                    }
                }
                if (funds.amount == 0)
                {
                    ViewBag.msg = "请输入经费总额。";
                    return View(funds);
                }
                if (funds.balance == null || funds.balance == 0)
                {
                    //自动设置余额
                    decimal usedfunds = (from fs in db.Funds
                                  join bill in db.Reimbursement
                                  on fs.f_id equals bill.r_funds_id
                                         join u in db.User_Info
                                  on fs.f_manager equals u.user_id into T1
                                  from t1 in T1.DefaultIfEmpty()
                                  where fs.f_id == funds.id && bill.r_add_user_id == user && bill.r_bill_state == 1
                                  select bill.r_fact_amount).DefaultIfEmpty(0).Sum();
                    if (usedfunds > 0) funds.balance = funds.amount - usedfunds;
                    if (funds.balance < 0)
                    {
                        ViewBag.msg = "出错：当前设置的经费总额小于已使用的经费总额。";
                        return View(funds);
                    }
                }
                funds.toDBModel(model);
                db.Entry(model).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    StringBuilder errors = new StringBuilder();
                    IEnumerable<DbEntityValidationResult> validationResult = ex.EntityValidationErrors;
                    foreach (DbEntityValidationResult result in validationResult)
                    {
                        ICollection<DbValidationError> validationError = result.ValidationErrors;
                        foreach (DbValidationError err in validationError)
                        {
                            errors.Append(err.PropertyName + ":" + err.ErrorMessage + "\r\n");
                        }
                    }
                    ErrorUnit.WriteErrorLog(errors.ToString(), this.GetType().Name);
                    ViewBag.msg = " 经费信息更新失败。";
                    return View(funds);
                }
                ViewBag.msg = "经费信息修改成功。";
                Sys_Log log = new Sys_Log()
                {
                    log_content = "修改经费信息" + model.f_name,
                    log_ip = Common.IpHelper.GetIP(),
                    log_target = funds.id.ToString(),
                    log_time = DateTime.Now,
                    log_type = 7,
                    log_user_id = user,
                    log_device = ""
                };
                db.Sys_Log.Add(log);
                db.SaveChanges();
            }
            return View(funds);
        }
        [HttpPost]
        public JsonResult Delete(string fid)
        {
            int id = PageValidate.FilterParam(fid);
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(user, db, "添加经费", "经费管理"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "paramErr";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
                if (id == 0)
            {
                json.msg_text = "参数传递失败，请重试。";
                json.msg_code = "paramErr";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            Funds funds = db.Funds.Find(id);
            if (funds == null)
            {
                json.msg_text = "没有找到该经费，该经费可能已被删除。";
                json.msg_code = "noThis";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            if(user!=funds.f_manager&& !RoleCheck.CheckHasAuthority(user, db, "经费管理"))
            {
                json.msg_text = "非经费管理员不能操作他们经费。";
                json.msg_code = "paramErr";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            var used = (from bill in db.Reimbursement
                        where bill.r_funds_id == funds.f_id && bill.r_bill_state == 1
                        select 1).Count();
            if (used > 0)
            {
                json.msg_text = "该经费已在使用中，无法删除。";
                json.msg_code = "inUsed";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            db.Funds.Remove(funds);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                json.msg_text = "删除失败，请重新操作。";
                json.msg_code = "delErr";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            json.msg_text = "删除成功。";
            json.msg_code = "success";
            json.state = 1;
            return Json(json, JsonRequestBehavior.AllowGet);
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
