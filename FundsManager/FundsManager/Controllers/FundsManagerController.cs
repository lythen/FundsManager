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
using System.Data.Entity.Validation;
using System.Text;
using FundsManager.Common;
using FundsManager.Common.DEncrypt;

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
                              applyamount = (
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
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            SetSelect();
            FundsModel funds = (from f in db.Funds
                                where f.f_id == (int)id
                                select new FundsModel
                                {
                                    amount = f.f_amount,
                                    id = f.f_id,
                                    balance = f.f_balance,
                                    expireDate = f.f_expireDate,
                                    info = f.f_info,
                                    manager = f.f_manager,
                                    name = f.f_name,
                                    source = f.f_source,
                                    state = f.f_state,
                                    year = f.f_in_year
                                }).FirstOrDefault();
            if (funds == null)
            {
                return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有找到该经费。" });
            }
            if (user != funds.manager) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有对该经费的管理权限。" });
            return View(funds);
        }

        // POST: FundsManager/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,expireDate,source,amount,balance,manager,info,state")] FundsModel funds)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            if (ModelState.IsValid)
            {
                SetSelect();
                Funds model = db.Funds.Find(funds.id);
                if (funds == null)
                {
                    ViewBag.msg = "没有找到该经费。";
                    return View(funds);
                }
                if (user != model.f_manager)
                {
                    ViewBag.msg = "您不是该经费的管理员，没有更改权限。";
                    return View(funds);
                }
                if (model.f_name != funds.name)
                    if (db.Funds.Where(x => x.f_name == funds.name.ToString() && x.f_id != funds.id).Count() > 0)
                    {
                        ViewBag.msg = "该名称已被使用";
                        return View(funds);
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
            var used = (from apply in db.Funds_Apply
                        join ac in db.Funds_Apply_Child
                        on apply.apply_number equals ac.c_apply_number
                        where ac.c_funds_id == funds.f_id && ac.c_state == 1
                        select 1).Count();
            if (used > 0)
            {
                json.msg_text = "该经费已在使用中，无法删除。";
                json.msg_code = "inUsed";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            //db.Funds.Remove(funds);
            Recycle_Funds rf = new Recycle_Funds();
            rf.f_amount = funds.f_amount;
            rf.f_balance = funds.f_balance;
            rf.f_delete_time = DateTime.Now;
            rf.f_delete_user = user;
            rf.f_expireDate = funds.f_expireDate;
            rf.f_id = funds.f_id;
            rf.f_info = funds.f_info;
            rf.f_in_year = funds.f_in_year;
            rf.f_manager = funds.f_manager;
            rf.f_name = funds.f_name;
            rf.f_source = funds.f_source;
            rf.f_state = funds.f_state;
            db.Funds_Recycle.Add(rf);
            try
            {
                db.SaveChanges();
            }catch(Exception e)
            {
                json.msg_text = "删除失败，请重新操作。";
                json.msg_code = "recyErr";
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
        #region 批复流程管理
        public ActionResult Process()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            var list = (from pro in db.Process_Info
                        join u in db.User_Info on pro.process_user_id equals u.user_id
                        select new ProcessModel
                        {
                            id = pro.process_id,
                            name = pro.process_name,
                            time = pro.process_create_time,
                            user = u.real_name,
                            isSelf = pro.process_user_id== user,
                            funds = pro.process_funds
                        }).ToList();
            return View(list);
        }
        public ActionResult ProcessAdd()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            List<SelectOption> options = DropDownList.FundsManagerSelect();
            ViewBag.Users = DropDownList.SetDropDownList(options);
            ProcessModel model = new ProcessModel();
            model.processList = new List<ProcessDetail>();
            model.processList.Add(new ProcessDetail() { sort=1 });
            return View(model);
        }
        [HttpPost]
        public JsonResult ProcessAdd(ProcessModel model)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                goto next;
            }
            if (model.processList == null || model.processList.Count() == 0)
            {
                json.msg_code = "noList";
                json.msg_text = "不包含任何流程。";
                goto next;
            }
            string proName= PageValidate.InputText(model.name, 50);
            if (db.Process_Info.Where(x => x.process_name == proName).Count() > 0)
            {
                json.msg_code = "nameExists";
                json.msg_text = "该名称已存在。";
                goto next;
            }
            
            int user = PageValidate.FilterParam(User.Identity.Name);
            Process_Info info = new Process_Info();
            info.process_create_time = DateTime.Now;
            info.process_name = PageValidate.InputText(model.name, 50);
            info.process_user_id = user;
            db.Process_Info.Add(info);
            try
            {
                db.SaveChanges();
            }
            catch
            {
                json.msg_code = "infoError";
                json.msg_text = "流程信息添加失败。";
                goto next;
            }
            int i = 1;
            foreach(ProcessDetail item in model.processList.OrderBy(x => x.sort))
            {
                Process_List list = new Process_List();
                list.po_process_id = info.process_id;
                list.po_sort = i;
                list.po_user_id = item.user;
                i++;
                db.Process_List.Add(list);
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception e) {
                ErrorUnit.WriteErrorLog(e.ToString(), "WriteProcessList");
                json.msg_code = "infoError";
                json.msg_text = "流程列表添加失败。";
                goto next;
            }
            json.msg_code = "success";
            json.state = 1;
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ProcessEdit(int? id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            List<SelectOption> options = DropDownList.FundsManagerSelect();
            ViewBag.Users = DropDownList.SetDropDownList(options);
            ProcessModel model = getProcessModel((int)id);
            if (model == null||model.uid!=user)
            {
                ViewBag.mag = "该流程不存在或者您非该流程创建者。";
                model = new ProcessModel();
                model.processList = new List<ProcessDetail>();
                model.processList.Add(new ProcessDetail() { sort = 1 });
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult ProcessEdit(ProcessModel model)
        {
            return View();
        }
        public JsonResult getProcessModel(string pid)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                goto next;
            }
            int id = PageValidate.FilterParam(pid);
            ProcessModel model = getProcessModel(id);
            if (model == null)
            {
                json.msg_text = "该流程不存在或已被删除。";
                json.msg_code = "notExists";
                goto next;
            }
            foreach(var item in model.processList)
            {
                item.strUser = AESEncrypt.Decrypt(item.strUser);
            }
            json.state = 1;
            json.data = model;
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        ProcessModel getProcessModel(int id)
        {
            var model = (from pro in db.Process_Info
                         join u in db.User_Info on pro.process_user_id equals u.user_id
                         where pro.process_id == id
                         select new ProcessModel
                         {
                             name = pro.process_name,
                             uid=pro.process_user_id,
                             processList = (from pl in db.Process_List
                                            join u2 in db.User_Info on pl.po_user_id equals u2.user_id
                                            where pl.po_process_id == pro.process_id
                                            select new ProcessDetail
                                            {
                                                id = pl.po_id,
                                                sort = pl.po_sort,
                                                strUser = u2.real_name,
                                                user=pl.po_user_id
                                            }
                                  ).ToList()
                         }).FirstOrDefault();
            return model;
        }
        #endregion
        #region 统计
        public void setSearchSelect()
        {

        }
        public ActionResult Statistics()
        {
            FundsSearchModel info = new FundsSearchModel();
            return View(info);
        }
        [HttpPost]
        public ActionResult Statistics(FundsSearchModel info)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);

            setSearchSelect();
            FundsStatistics model = new FundsStatistics();
            switch (info.statorDetail)
            {
                case 0:model.stats = getStatistics(info);break;
                case 1:model.details = getStatisticsDetail(info);break;
            }
            ViewData["StatData"] = model;
            return View(info);
        }
        List<FundsStatDetail> getStatisticsDetail(FundsSearchModel info)
        {
            var query = (from capply in db.Funds_Apply_Child
                         join apply in db.Funds_Apply on capply.c_apply_number equals apply.apply_number
                         join user in db.User_Info on apply.apply_user_id equals user.user_id
                         join funds in db.Funds on capply.c_funds_id equals funds.f_id
                         select new FundsStatDetail
                         {
                             applyAmount = capply.c_amount,
                             applyTime = apply.apply_time,
                             fname = funds.f_name,
                             uname = user.user_name
                         });
            info.Amount = query.Count();
            return query.Skip(info.PageSize * (info.PageIndex - 1)).Take(info.PageSize).ToList();
        }
        List<FundsStat> getStatistics(FundsSearchModel info)
        {
            var query = (from funds in db.Funds
                         where funds.f_in_year==info.year.ToString()
                         select new FundsStat
                         {
                              amount=funds.f_amount,
                               name=funds.f_name
                         }
                       ).ToList();
            return null;
        }
        #endregion
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
