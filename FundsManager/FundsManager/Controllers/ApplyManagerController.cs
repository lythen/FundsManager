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
using FundsManager.Common;
using FundsManager.Common.DEncrypt;
using System.Data.Entity.Validation;
using System.Text;

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
                            where apply.apply_user_id == user && apply.apply_state != 3
                            select new ApplyListModel
                            {
                                amount = apply.apply_amount,
                                number = apply.apply_number,
                                state = apply.apply_state,
                                time = apply.apply_time,
                                childState = (from child in db.Funds_Apply_Child
                                              join das in db.Dic_Apply_State on apply.apply_state equals das.das_state_id
                                              where child.c_apply_number==apply.apply_number
                                              select new ChildState { childState=child.c_child_number + "," + das.das_state_name + "," + child.c_amount }
                                              )
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
            Funds_Apply funds_Apply = db.Funds_Apply.Where(x => x.apply_state == 1 && x.apply_number == id).FirstOrDefault();
            if (funds_Apply == null)
            {
                return HttpNotFound();
            }
            var cmList = (from child in db.Funds_Apply_Child
                          join funds in db.Funds
                          on child.c_funds_id equals funds.f_id
                          join user in db.User_Info
                          on funds.f_manager equals user.user_id into T1
                          from t1 in T1
                          join das in db.Dic_Apply_State on child.c_state equals das.das_state_id
                          where child.c_apply_number == id
                          select new ApplyFundsManager
                          {
                              Cnumber = child.c_child_number,
                              strManager = t1.real_name,
                              strState = das.das_state_name
                          }
                          ).ToList();
            if (cmList.Count() > 0) ViewBag.number = id;
            foreach (ApplyFundsManager item in cmList)
            {
                item.strManager = AESEncrypt.Decrypt(item.strManager);
            }
            return View(cmList);
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
        public JsonResult Create(ApplyEditModel funds_Apply)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_code = "nologin";
                goto next;
            }
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            SetSelect();
            if (ModelState.IsValid)
            {


                Funds_Apply apply = new Funds_Apply();
                apply.apply_amount = funds_Apply.amount;
                apply.apply_state = 0;
                apply.apply_time = DateTime.Now;
                apply.apply_user_id = user;
                var maxfa = db.Funds_Apply.OrderByDescending(x => x.apply_number).FirstOrDefault();
                //apply_number:年份+10001自增
                if (maxfa == null) apply.apply_number = DateTime.Now.Year.ToString() + "10001";
                else apply.apply_number = DateTime.Now.Year.ToString() + (int.Parse(maxfa.apply_number.Substring(4)) + 1);
                db.Funds_Apply.Add(apply);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    json.msg_code = "error";
                    json.msg_text = "申请单提交失败。";
                    goto next;
                }
                //子申请单号由申请单号+3位序号如 201710001-001
                int i = 1;
                foreach (ApplyChildModel citem in funds_Apply.capply)
                {
                    Funds_Apply_Child capply = new Funds_Apply_Child();
                    capply.c_apply_number = apply.apply_number;
                    capply.c_child_number = string.Format("{0}-{1}", apply.apply_number, GetIntStr(i));
                    capply.c_amount = citem.amount;
                    capply.c_apply_for = citem.applyFor;
                    capply.c_funds_id = citem.Fid;
                    capply.c_state = 0;
                    db.Funds_Apply_Child.Add(capply);
                    i++;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    var list = db.Funds_Apply_Child.Where(x => x.c_apply_number == apply.apply_number);
                    if (list.Count() > 0) db.Funds_Apply_Child.RemoveRange(list);
                    db.Funds_Apply.Remove(apply);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch { }
                    json.msg_code = "error";
                    json.msg_text = "申请单提交失败。";
                    goto next;
                }
                //db.Funds_Apply.Add(funds_Apply);
                //db.SaveChanges();
                json.state = 1;
                json.msg_code = "success";
                json.msg_text = apply.apply_number;
            }
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        string GetIntStr(int num)
        {
            if (num < 10) return "00" + num;
            if (num < 100) return "0" + num;
            return num.ToString();
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
        [HttpPost]
        // GET: ApplyManager/Delete/5
        public JsonResult Delete(string cnumber)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_code = "nologin";
                goto next;
            }
            if (cnumber == null)
            {
                json.msg_code = "errorNumber";
                json.msg_text = "申请单号获取失败。";
                goto next;
            }
            Funds_Apply_Child child = db.Funds_Apply_Child.Find(cnumber);
            if (child == null)
            {
                json.msg_code = "nodate";
                json.msg_text = "申请单不存在或被删除。";
                goto next;
            }
            if (child.c_state == 3)
            {
                json.msg_code = "forbidden";
                json.msg_text = "已批复同意的子申请单不允许删除。";
                goto next;
            }
            string number = child.c_apply_number;
            int c_num = db.Funds_Apply_Child.Where(x => x.c_apply_number == number).Count();
            if (c_num <= 1)
            {
                //只有一条子申请单情况，直接把父申请单也删除
                var f = db.Funds_Apply.Find(number);
                if (f != null)
                    if (f.apply_state == 3)
                    {
                        json.msg_code = "forbidden";
                        json.msg_text = "已批复同意的申请单不允许删除。";
                        goto next;
                    }
                    else
                        db.Funds_Apply.Remove(f);
            }
            db.Funds_Apply_Child.Remove(child);
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
                json.msg_code = "error";
                json.msg_text = "申请单删除失败。";
                goto next;
            }
            json.state = 1;
            json.msg_code = "success";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MyFunds()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            var waitList = (from apply in db.Funds_Apply
                            join das in db.Dic_Apply_State
                            on apply.apply_state equals das.das_state_id
                            where apply.apply_user_id == user && apply.apply_state == 3
                            select new ApplyListModel
                            {
                                amount = apply.apply_amount,
                                number = apply.apply_number,
                                state = apply.apply_state,
                                time = apply.apply_time,
                                childState = (from child in db.Funds_Apply_Child
                                              join das in db.Dic_Apply_State on apply.apply_state equals das.das_state_id
                                              where child.c_apply_number == apply.apply_number
                                              select new ChildState { childState = child.c_child_number + "," + das.das_state_name+","+child.c_amount }
                                              )
                            }
                            ).ToList();
            return View(waitList);
        }
        public ActionResult ApplyNext(string number)
        {
            if (number == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Funds_Apply funds_Apply = db.Funds_Apply.Where(x => x.apply_state == 1 && x.apply_number == number).FirstOrDefault();
            if (funds_Apply == null)
            {
                return HttpNotFound();
            }
            var cmList = (from child in db.Funds_Apply_Child
                          join funds in db.Funds
                          on child.c_funds_id equals funds.f_id
                          join user in db.User_Info 
                          on funds.f_manager equals user.user_id into T1
                          from t1 in T1
                          join das in db.Dic_Apply_State on child.c_state equals das.das_state_id
                          where child.c_apply_number == number
                          select new ApplyFundsManager
                          {
                              Cnumber = child.c_child_number,
                              strManager = t1.real_name,
                              strState=das.das_state_name
                          }
                          ).ToList();
            if (cmList.Count() > 0) ViewBag.number = number;
            foreach(ApplyFundsManager item in cmList)
            {
                item.strManager = AESEncrypt.Decrypt(item.strManager);
            }
            return View(cmList);
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
