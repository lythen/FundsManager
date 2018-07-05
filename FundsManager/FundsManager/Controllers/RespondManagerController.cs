using System.Linq;
using System.Web.Mvc;
using FundsManager.DAL;
using FundsManager.Models;
using FundsManager.Common;
using FundsManager.ViewModels;
using FundsManager.Common.DEncrypt;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity.Validation;

namespace FundsManager.Controllers
{
    public class RespondManagerController : Controller
    {
        private FundsContext db = new FundsContext();

        // GET: RespondManager
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "Index" });
            ViewData["ViewUsers"] = DropDownList.RespondUserSelect();
            int userId = PageValidate.FilterParam(User.Identity.Name);
            var list = getResponseDetail(userId, 0);
            return View(list);
        }
        public ActionResult Responded()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "Index" });
            int userId = PageValidate.FilterParam(User.Identity.Name);
            var list = getResponseDetail(userId, 1, 2, 3, 4);
            return View(list);
        }
        List<ResponseDetail> getResponseDetail(int userId, params int[] state)
        {
            var list = (from pr in db.Process_Respond
                        join bill in db.Reimbursement on pr.pr_reimbursement_code equals bill.reimbursement_code
                        join user in db.User_Info on bill.r_add_user_id equals user.user_id into T3
                        from t3 in T3.DefaultIfEmpty()
                        join s in db.Dic_Respond_State on pr.pr_state equals s.drs_state_id into T4
                        from t4 in T4.DefaultIfEmpty()
                        join f in db.Funds on bill.r_funds_id equals f.f_id
                        where pr.pr_user_id == userId && state.Contains(pr.pr_state)
                        select new ResponseDetail
                        {
                            reimbursementCode = bill.reimbursement_code,
                            content = pr.pr_content,
                            id = pr.pr_id,
                            number = pr.pr_number,
                            strState = t4.drs_state_name,
                            state = pr.pr_state,
                            time = pr.pr_time,
                            applyUser = t3.real_name,
                            addDate = bill.r_add_date,
                            name = bill.reimbursement_info,
                            fundsCode = f.f_code
                        }
                ).ToList();
            if (list != null)
                foreach (var item in list)
                {
                    item.applyUser = AESEncrypt.Decrypt(item.applyUser);
                }
            return list;
        }
        // GET: RespondManager/Details/5
        public JsonResult Details(int? id)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            if (id == null || id == 0)
            {
                json.msg_text = "参数传递失败，请重试。";
                json.msg_code = "paramErr";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            var apply = (from r in db.Process_Respond
                         join bill in db.Reimbursement on r.pr_reimbursement_code equals bill.reimbursement_code
                         join f in db.Funds on bill.r_funds_id equals f.f_id
                         join u in db.User_Info on bill.r_add_user_id equals u.user_id
                         where r.pr_id == id
                         select new ResponseDetail
                         {
                             applyUser = u.real_name,
                             reimbursementCode = bill.reimbursement_code,
                             fundsCode = f.f_code,
                             content = bill.reimbursement_info,
                             applyAmount = bill.r_bill_amount
                         }
                       ).FirstOrDefault();

            if (apply == null)
            {
                json.msg_text = "没找到该流程，可能已经撤销，请重试。";
                json.msg_code = "None";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            apply.applyUser = AESEncrypt.Decrypt(apply.applyUser);
            json.msg_code = "success";
            json.state = 1;
            json.data = apply;
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public JsonResult SetAgree(Respond respond)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_text = "没有登陆或登陆失效，请重新登陆后操作。";
                json.msg_code = "notLogin";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            int user = PageValidate.FilterParam(User.Identity.Name);
            if (respond.id == null || respond.id == 0)
            {
                json.msg_text = "参数传递失败，请重试。";
                json.msg_code = "paramErr";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            Process_Respond model = db.Process_Respond.Find(respond.id);
            if (model == null)
            {
                json.msg_text = "没找到该流程，可能已经撤销，请重试。";
                json.msg_code = "None";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            //批复当前流程
            int state = respond.agree == "yes" ? 1 : 2;
            model.pr_state = state;
            model.pr_time = DateTime.Now;
            model.pr_content = PageValidate.InputText(Server.UrlDecode(respond.reason), 2000);
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().ToString());
                json.msg_text = "操作失败，请重试。";
                json.msg_code = "Error";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            //是否为批复不通过
            Reimbursement bill = db.Reimbursement.Find(model.pr_reimbursement_code);
            if (bill == null)
            {
                json.msg_text = "操作失败，该报销单已被删除。";
                json.msg_code = "Error";
                return Json(json, JsonRequestBehavior.AllowGet);
            }

            if (state == 1)
            {
                //是否有next
                if (respond.next != null && respond.next != 0)
                {
                    Process_Respond pr = new Process_Respond();
                    pr.pr_reimbursement_code = model.pr_reimbursement_code;
                    pr.pr_user_id = (int)respond.next;
                    pr.pr_number = model.pr_number + 1;
                    db.Process_Respond.Add(pr);
                    db.SaveChanges();
                    model.next = pr.pr_id;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    Funds fmodel = db.Funds.Find(bill.r_funds_id);
                    if (fmodel == null)
                    {
                        json.msg_text = "所申请的经费已不存在，无法继续。";
                        json.msg_code = "applyError";
                        json.state = 0;
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                    if (fmodel.f_balance < bill.r_bill_amount)
                    {
                        //经费不足，回退批复
                        model.pr_state = 0;
                        model.pr_content = "当前经费余额不足";
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        json.msg_text = "当前经费余额不足，无法继续。";
                        json.msg_code = "applyError";
                        json.state = 0;
                    }
                    else
                    {
                        model.pr_state = state;
                        model.pr_content = respond.reason;
                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        fmodel.f_balance = fmodel.f_balance - bill.r_bill_amount;
                        db.Entry(fmodel).State = System.Data.Entity.EntityState.Modified;
                        bill.r_bill_state = state;
                        db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
                    }
                }
            }
            else
            {
                bill.r_bill_state = state;
                db.Entry(bill).State = System.Data.Entity.EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException et)
            {
                StringBuilder errors = new StringBuilder();
                IEnumerable<DbEntityValidationResult> validationResult = et.EntityValidationErrors;
                foreach (DbEntityValidationResult result in validationResult)
                {
                    ICollection<DbValidationError> validationError = result.ValidationErrors;
                    foreach (DbValidationError err in validationError)
                    {
                        errors.Append(err.PropertyName + ":" + err.ErrorMessage + "\r\n");
                    }
                }
                ErrorUnit.WriteErrorLog(errors.ToString(), this.GetType().Name);
                json.msg_text = "审核失败。";
                json.msg_code = "respondError";
                json.state = 0;
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            next:
            json.msg_text = "操作成功。";
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
