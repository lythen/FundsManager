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
        public ActionResult Index(BillsSearchModel info)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "Index" });
            int userId = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(userId, db, "批复管理", "批复")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限。" });
            if (RoleCheck.CheckHasAuthority(userId, db, "批复管理")) userId = 0;
            ApplyManager dal = new ApplyManager(db);
            ViewData["ViewUsers"] = DropDownList.RespondUserSelect();
            var list = getResponseDetail(userId, 0);
            ViewData["Bills"] = list;
            return View(info);
        }
        public ActionResult Responded(BillsSearchModel info)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "Index" });
            ApplyManager dal = new ApplyManager(db);
            int userId = PageValidate.FilterParam(User.Identity.Name);
            if (!RoleCheck.CheckHasAuthority(userId, db, "批复管理", "批复")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限。" });
            if (RoleCheck.CheckHasAuthority(userId, db, "批复管理")) userId = 0;
            ViewData["ViewUsers"] = DropDownList.RespondUserSelect();
            var list = getResponseDetail(userId, 1, 2, 3, 4);
            ViewData["Bills"] = list;
            return View(info);
        }
        List<ApplyListModel> getResponseDetail(int userId, params int[] state)
        {
            ApplyManager dal = new ApplyManager(db);
            var query = from pr in db.Process_Respond
                        join bill in db.Reimbursement on pr.pr_reimbursement_code equals bill.reimbursement_code
                        join user in db.User_Info on bill.r_add_user_id equals user.user_id
                        join s in db.Dic_Respond_State on bill.r_bill_state equals s.drs_state_id
                        join f in db.Funds on bill.r_funds_id equals f.f_id
                        where state.Contains(bill.r_bill_state)
                        orderby bill.r_add_date descending
                        select new ApplyListModel
                        {
                            amount = bill.r_bill_amount,
                            reimbursementCode = bill.reimbursement_code,
                            state = bill.r_bill_state,
                            strState = s.drs_state_name,
                            time = bill.r_add_date,
                            fundsCode = f.f_code,
                            fundsName = f.f_name,
                            userName = user.real_name,
                            info = bill.reimbursement_info,
                            userId = bill.r_add_user_id,
                            manager = pr.pr_user_id
                        };
            if (userId > 0) query = query.Where(x => x.manager == userId);
            var list = query.ToList();
            if (list != null)
                foreach (var item in list)
                {
                    item.attachmentsCount = dal.getAttachments(item.reimbursementCode,0).Count();
                    item.contents = dal.getContents(item.reimbursementCode, 0).ToList();
                    item.userName = AESEncrypt.Decrypt(item.userName);
                }
            
            return list;
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
            if (!RoleCheck.CheckHasAuthority(user, db, "批复管理", "批复"))
            {
                json.msg_text = "没有权限。";
                json.msg_code = "paramErr";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
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
            if (user != model.pr_user_id)
            {
                json.msg_text = "非该流程的当前批复人。";
                json.msg_code = "paramErr";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            var exists = db.Process_Respond.Where(x => x.pr_reimbursement_code == model.pr_reimbursement_code && x.pr_user_id == respond.next);
            if (exists.Count() > 0)
            {
                json.msg_text = "该审核人已存在审批列表中。";
                json.msg_code = "Exists";
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            //批复当前流程
            int state = respond.state;
            model.pr_state = state;
            model.pr_time = DateTime.Now;
            model.pr_content = PageValidate.InputText(Server.UrlDecode(respond.reason), 2000);
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;

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
                Funds fmodel = db.Funds.Find(bill.r_funds_id);
                if (fmodel == null)
                {
                    json.msg_text = "所申请的经费已不存在，无法继续。";
                    json.msg_code = "applyError";
                    json.state = 0;
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
                //是否有next
                if (respond.next != null && respond.next != 0)
                {
                    Process_Respond pr = new Process_Respond();
                    pr.pr_reimbursement_code = model.pr_reimbursement_code;
                    pr.pr_user_id = (int)respond.next;
                    pr.pr_number = model.pr_number + 1;
                    db.Process_Respond.Add(pr);
                    model.next = pr.pr_id;
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    if (fmodel.f_balance < bill.r_bill_amount)
                    {
                        //经费不足，回退批复
                        json.msg_text = "当前经费余额不足，无法继续。";
                        json.msg_code = "applyError";
                        json.state = 0;
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        fmodel.f_balance = fmodel.f_balance - bill.r_bill_amount;
                        db.Entry(fmodel).State = System.Data.Entity.EntityState.Modified;
                        bill.r_bill_state = state;
                        bill.r_fact_amount = bill.r_bill_amount;
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
