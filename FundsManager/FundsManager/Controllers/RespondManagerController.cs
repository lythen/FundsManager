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
            int id = PageValidate.FilterParam(User.Identity.Name);
            var list = (from pr in db.Process_Respond
                        join ca in db.Funds_Apply_Child on pr.pr_apply_number equals ca.c_child_number into T1
                        from t1 in T1.DefaultIfEmpty()
                        join a in db.Funds_Apply on t1.c_apply_number equals a.apply_number into T2
                        from t2 in T2.DefaultIfEmpty()
                        join user in db.User_Info on t2.apply_user_id equals user.user_id into T3
                        from t3 in T3.DefaultIfEmpty()
                        join s in db.Dic_Respond_State on pr.pr_state equals s.drs_state_id into T4
                        from t4 in T4.DefaultIfEmpty()
                        join f in db.Funds on t1.c_funds_id equals f.f_id
                        where pr.pr_user_id == id && pr.pr_state == 0
                        select new ResponseDetail
                        {
                            capply_number = pr.pr_apply_number,
                            content = pr.pr_content,
                            id = pr.pr_id,
                            number = pr.pr_number,
                            strState = t4.drs_state_name,
                            state = pr.pr_state,
                            time = pr.pr_time,
                            apply_number = t2.apply_number,
                            applyUser = t3.real_name,
                            addDate = t2.apply_time,
                            name = t1.c_apply_for,
                            fundsCode = f.f_code
                        }
                ).ToList();
            if (list != null)
                foreach (var item in list)
                {
                    item.applyUser = AESEncrypt.Decrypt(item.applyUser);
                }
            return View(list);
        }
        public ActionResult Responded()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "Index" });
            int id = PageValidate.FilterParam(User.Identity.Name);
            var list = (from pr in db.Process_Respond
                        join ca in db.Funds_Apply_Child on pr.pr_apply_number equals ca.c_child_number into T1
                        from t1 in T1.DefaultIfEmpty()
                        join a in db.Funds_Apply on t1.c_apply_number equals a.apply_number into T2
                        from t2 in T2.DefaultIfEmpty()
                        join user in db.User_Info on t2.apply_user_id equals user.user_id into T3
                        from t3 in T3.DefaultIfEmpty()
                        join s in db.Dic_Respond_State on pr.pr_state equals s.drs_state_id into T4
                        from t4 in T4.DefaultIfEmpty()
                        join f in db.Funds on t1.c_funds_id equals f.f_id into T5
                        from t5 in T5.DefaultIfEmpty()
                        where pr.pr_user_id == id && pr.pr_state != 0
                        select new ResponseDetail
                        {
                            capply_number = pr.pr_apply_number,
                            content = pr.pr_content,
                            id = pr.pr_id,
                            number = pr.pr_number,
                            strState = t4.drs_state_name,
                            state = pr.pr_state,
                            time = pr.pr_time,
                            apply_number = t2.apply_number,
                            applyUser = t3.real_name,
                            addDate = t2.apply_time,
                            name = t1.c_apply_for,
                            fundsCode = t5.f_code
                        }
                ).ToList();
            if (list != null)
                foreach (var item in list)
                {
                    item.applyUser = AESEncrypt.Decrypt(item.applyUser);
                }
            return View(list);
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
                         join c in db.Funds_Apply_Child on r.pr_apply_number equals c.c_child_number
                         join f in db.Funds on c.c_funds_id equals f.f_id
                         join a in db.Funds_Apply on c.c_apply_number equals a.apply_number
                         join u in db.User_Info on a.apply_user_id equals u.user_id
                         where r.pr_id == id
                         select new ResponseDetail
                         {
                             applyUser = u.real_name,
                             apply_number = a.apply_number,
                             capply_number = c.c_child_number,
                             fundsCode = f.f_code,
                             content = c.c_apply_for,
                             applyAmount = c.c_amount
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
            Funds_Apply_Child acm;
            Funds_Apply fam;
            if (state == 2)
            {
                //一个不通过，全部不通过。
                acm = db.Funds_Apply_Child.Find(model.pr_apply_number);
                fam = db.Funds_Apply.Find(acm.c_apply_number);
                if (acm != null&& fam!=null)
                {
                    //批复不通过
                    var allList = (from pr in db.Process_Respond
                                   join c in db.Funds_Apply_Child on pr.pr_apply_number equals c.c_child_number
                                   where c.c_apply_number==fam.apply_number
                                   select pr).ToList();
                    foreach (var item in allList)
                    {
                        item.pr_state = 2;
                        item.pr_time = DateTime.Now;
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                    //子单不通过
                    var childs = (from cs in db.Funds_Apply_Child where cs.c_apply_number == acm.c_apply_number orderby cs.c_funds_id select cs).ToList();
                    foreach(var item in childs)
                    {
                        item.c_state = 2;
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                    
                    //主单不通过
                    
                    if (fam != null)
                    {
                        fam.apply_state = 2;
                        db.Entry(fam).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                try
                {
                    db.SaveChanges();
                }catch(DbEntityValidationException et)
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
                //结束
                goto next;
            }
            //是否有next
            if (respond.next != null && respond.next != 0)
            {
                Process_Respond pr = new Process_Respond();
                pr.pr_apply_number = model.pr_apply_number;
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
                //检查是否已经全部批复完成
                int pstate = 0;
                var allList = (from pr in db.Process_Respond
                               where pr.pr_apply_number == model.pr_apply_number
                               orderby pr.pr_number ascending
                               select pr).ToList();
                foreach (var item in allList)
                {
                    pstate = item.pr_state;
                    if (item.pr_state == 0)
                    {//未完成，结束
                        break;
                    }
                    if (item.pr_state == 1)
                    {//批复通过，查下一个批复
                        continue;
                    }
                    if (item.pr_state == 2)
                    {//批复不通过，直接跳出来
                        break;
                    }
                }
                if (pstate != 0)
                {
                    acm = db.Funds_Apply_Child.Find(model.pr_apply_number);
                    if (acm == null)
                    {
                        json.msg_text = "流程的子申请单已被删除，无法继续。";
                        json.msg_code = "capplyError";
                        json.state = 0;
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                    fam = db.Funds_Apply.Find(acm.c_apply_number);
                    if (fam == null)
                    {
                        json.msg_text = "主申请单已被删除，无法继续。";
                        json.msg_code = "applyError";
                        json.state = 0;
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                    acm.c_state = pstate;
                    //必需主单号为批复通过才能操作
                    //从经费中减去金额
                    if (acm.c_state == 1)//通过
                    {
                        //查找所有子单号是否全部通过
                        var childs = (from cs in db.Funds_Apply_Child where cs.c_apply_number == acm.c_apply_number orderby cs.c_funds_id select cs).ToList();
                        int astate = 0;
                        foreach (var item in childs)
                        {
                            astate = item.c_state;
                            if (astate == 0)
                            {//未完成，结束
                                break;
                            }
                            if (astate == 1)
                            {//批复通过，查下一个批复
                                continue;
                            }
                            if (astate == 2)
                            {//批复不通过，直接跳出来
                                break;
                            }
                        }
                        if (astate != 0)
                        {
                            fam.apply_state = astate;
                            if (astate == 1)
                            {
                                Funds fmodel = null;
                                decimal fee = 0;
                                int fid = 0;
                                foreach (var item in childs)
                                {
                                    if (fid != item.c_funds_id)
                                    {
                                        if (fid != 0)
                                        {
                                            fmodel = db.Funds.Find(fid);
                                            if (fmodel == null)
                                            {
                                                json.msg_text = "所申请的经费已不存在，无法继续。";
                                                json.msg_code = "applyError";
                                                json.state = 0;
                                                return Json(json, JsonRequestBehavior.AllowGet);
                                            }
                                            if (fmodel.f_balance < fee)
                                            {
                                                //经费不足，回退批复
                                                model.pr_state = 0;
                                                model.pr_content = "当前经费余额不足";
                                                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                                                db.SaveChanges();

                                                json.msg_text = "当前经费余额不足，无法继续。";
                                                json.msg_code = "applyError";
                                                json.state = 0;
                                                return Json(json, JsonRequestBehavior.AllowGet);
                                            }
                                            fmodel.f_balance = fmodel.f_balance - fee;
                                            db.Entry(fmodel).State = System.Data.Entity.EntityState.Modified;
                                        }
                                        fid = item.c_funds_id;
                                        fee = 0;
                                    }
                                    fee += item.c_amount;
                                }
                                if (fid != 0)
                                {
                                    //加入最后的
                                    fmodel = db.Funds.Find(fid);
                                    if (fmodel == null)
                                    {
                                        json.msg_text = "所申请的经费已不存在，无法继续。";
                                        json.msg_code = "applyError";
                                        json.state = 0;
                                        return Json(json, JsonRequestBehavior.AllowGet);
                                    }
                                    if (fmodel.f_balance < acm.c_amount)
                                    {
                                        //经费不足，回退批复
                                        model.pr_state = 0;
                                        model.pr_content = "当前经费余额不足";
                                        db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();

                                        json.msg_text = "当前经费余额不足，无法继续。";
                                        json.msg_code = "applyError";
                                        json.state = 0;
                                        return Json(json, JsonRequestBehavior.AllowGet);
                                    }
                                    fmodel.f_balance = fmodel.f_balance - acm.c_amount;
                                    db.Entry(fmodel).State = System.Data.Entity.EntityState.Modified;
                                }
                                
                            }
                        }

                    }
                    db.Entry(acm).State = System.Data.Entity.EntityState.Modified;
                    //整个申请完成
                    db.Entry(fam).State = System.Data.Entity.EntityState.Modified;
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

                }
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
