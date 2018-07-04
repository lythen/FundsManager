﻿using System;
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
using System.Configuration;
using System.IO;

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
            //return RedirectToAction("myFunds");
            var waitList = (from bill in db.Reimbursement
                            join s in db.Dic_Respond_State on bill.r_bill_state equals s.drs_state_id
                            where bill.r_add_user_id == user && bill.r_bill_state != 3
                            select new ApplyListModel
                            {
                                amount = bill.r_bill_amount,
                                number = bill.reimbursement_code,
                                state = bill.r_bill_state,
                                time = bill.r_add_time,
                                strState = s.drs_state_name,
                                child = (from content in db.Reimbursement_Content
                                         where child.c_apply_number == bill.apply_number
                                         select new ApplyChildModel
                                         {
                                             Cnumber = child.c_child_number,
                                             fundsCode = f.f_code,
                                             amount = child.c_amount,
                                             strState = das.drs_state_name,
                                             factGet = child.c_get
                                         }
                                              ).ToList()
                            }
                            ).ToList();
            return View(waitList);
        }

        // GET: ApplyManager/Details/5
        public ActionResult Details(string id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reimbursement funds_Apply = db.Funds_Apply.Where(x => x.apply_number == id).FirstOrDefault();
            if (funds_Apply == null)
            {
                return HttpNotFound();
            }
            var cmList = (from child in db.Funds_Apply_Child
                          join funds in db.Funds
                          on child.c_funds_id equals funds.f_id
                          join das in db.Dic_Respond_State on child.c_state equals das.drs_state_id
                          where child.c_apply_number == id
                          select new ApplyFundsManager
                          {
                              Cnumber = child.c_child_number,
                              strState = das.drs_state_name
                          }
                          ).ToList();
            if (cmList.Count() > 0) ViewBag.number = id;
            foreach (ApplyFundsManager item in cmList)
            {
                var plist = (from p in db.Process_Respond
                             join s in db.Dic_Respond_State
                             on p.pr_state equals s.drs_state_id into T1
                             from t1 in T1.DefaultIfEmpty()
                             join u in db.User_Info
                             on p.pr_user_id equals u.user_id into T2
                             from t2 in T2.DefaultIfEmpty()
                             where p.pr_apply_number == item.Cnumber
                             select new ListResponseModel
                             {
                                 capply_number = p.pr_apply_number,
                                 content = p.pr_content,
                                 id = p.pr_id,
                                 number = p.pr_number,
                                 strState = t1.drs_state_name,
                                 state = p.pr_state,
                                 time = p.pr_time,
                                 user = t2.user_name
                             }
                             ).ToList();
                item.processList = plist;
            }
            return View(cmList);
        }
        // GET: ApplyManager/Create
        public ActionResult Create()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            SetSelect(0);
            return View();
        }

        // POST: ApplyManager/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public JsonResult Create(ApplyListModel _sbill)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_code = "nologin";
                goto next;
            }
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            SetSelect(0);
            if (ModelState.IsValid)
            {
                var funds = (from fs in db.Funds
                             where fs.f_id == _sbill.Fid
                             select fs).FirstOrDefault();
                if (funds.f_amount == 0)
                {
                    json.msg_code = "error";
                    json.msg_text = string.Format("报销单提交失败,id为{0}的经费没有设置总额。", _sbill.Fid);
                    goto next;
                }
                if (funds.f_balance < _sbill.amount)
                {
                    json.msg_code = "error";
                    json.msg_text = string.Format("报销单提交失败,id为{0}的经费不足。", _sbill.Fid);
                    goto next;
                }

                Reimbursement bill = new Reimbursement();
                bill.r_bill_amount = _sbill.amount;
                bill.r_bill_state = 0;
                bill.r_add_time = DateTime.Now;
                bill.r_add_user_id = user;
                var maxfa = db.Reimbursement.OrderByDescending(x => x.reimbursement_code).FirstOrDefault();
                //apply_number:年份+10001自增
                if (maxfa == null) bill.reimbursement_code = DateTime.Now.Year.ToString() + "10001";
                else bill.reimbursement_code = DateTime.Now.Year.ToString() + (int.Parse(maxfa.reimbursement_code.Substring(4)) + 1);
                db.Reimbursement.Add(bill);
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
                //添加报销内容
                foreach (ViewContentModel citem in _sbill.contents)
                {
                    Reimbursement_Content content = new Reimbursement_Content();
                    content.c_reimbursement_code = bill.reimbursement_code;
                    content.c_amount = citem.amount;
                    db.Reimbursement_Content.Add(content);
                    //添加明细

                }

                //添加附件
                StringBuilder sbErr = new StringBuilder();
                if (_sbill.attachments != null && _sbill.attachments.Count() > 0)
                {
                    string attachment_path = ConfigurationManager.AppSettings["attachmentPath"];
                    string attachment_temp_path = ConfigurationManager.AppSettings["attachmentTempPath"];
                    if (!Directory.Exists(attachment_path)) Directory.CreateDirectory(attachment_path);
                    string filePath,tempFile,saveFileName;
                    foreach (string file in _sbill.attachments)
                    {
                        try
                        {
                            saveFileName = string.Format("{0}\\{1}\\{2}", bill.reimbursement_code, DateTime.Now.ToString("yyyyMMdd"), file);
                            tempFile = attachment_temp_path + file;
                            filePath = string.Format("{0}{1}", attachment_path, saveFileName);
                            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                            System.IO.File.Move(tempFile, filePath);
                        }
                        catch
                        {
                            sbErr.Append("文件【").Append(file).Append("】保存失败，请重新上传");
                            continue;
                        }
                        Reimbursement_Attachment attachment = new Reimbursement_Attachment
                        {
                            attachment_path = saveFileName,
                            atta_detail_id = 0,
                            atta_reimbursement_code = bill.reimbursement_code
                        };
                        db.Reimbursement_Attachment.Add(attachment);
                    }
                }
                //添加批复人
                Process_Respond pr = new Process_Respond();
                pr.pr_reimbursement_code = bill.reimbursement_code;
                pr.pr_user_id = _sbill.next;
                pr.pr_number = 1;
                db.Process_Respond.Add(pr);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Delete(bill.reimbursement_code);
                    json.msg_code = "error";
                    json.msg_text = "申请单提交失败。";
                    goto next;
                }
                json.state = 1;
                json.msg_code = "success";
                json.msg_text = bill.reimbursement_code; 
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
        void SetSelect(int user)
        {

            List<SelectOption> options = DropDownList.FundsSelect(user);
            ViewData["Funds"] = DropDownList.SetDropDownList(options);
            options = DropDownList.ContentSelect();
            ViewData["Contents"] = DropDownList.SetDropDownList(options);
            ViewData["ViewUsers"] = DropDownList.RespondUserSelect();
        }
        // GET: ApplyManager/Edit/5
        public ActionResult Edit(string id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SetSelect(0);
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            ApplyListModel funds_Apply = (from apply in db.Funds_Apply
                                          join das in db.Dic_Respond_State
                                          on apply.apply_state equals das.drs_state_id
                                          where apply.apply_number==id
                                          select new ApplyListModel
                                          {
                                              amount = apply.apply_amount,
                                              number = apply.apply_number,
                                              state = apply.apply_state,
                                              strState = das.drs_state_name,
                                              time = apply.apply_time,
                                              child = (from c in db.Funds_Apply_Child
                                                       join das in db.Dic_Respond_State on c.c_state equals das.drs_state_id
                                                       join f in db.Funds on c.c_funds_id equals f.f_id
                                                       where c.c_apply_number == apply.apply_number
                                                       select new ApplyChildModel
                                                       {
                                                           Cnumber = c.c_child_number,
                                                           fundsCode = f.f_code,
                                                           amount = c.c_amount,
                                                           strState = das.drs_state_name,
                                                           factGet = c.c_get,
                                                           applyFor = c.c_apply_for,
                                                           Fid = c.c_funds_id,
                                                           getInfo = c.c_get_info,
                                                           state = c.c_state
                                                       }
                                                            ).ToList()
                                          }
                            ).FirstOrDefault();
            return View(funds_Apply);
        }

        // POST: ApplyManager/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public JsonResult Edit(ApplyListModel funds_Apply)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_code = "nologin";
                goto next;
            }
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            SetSelect(0);
            if (ModelState.IsValid)
            {
                Reimbursement apply = db.Funds_Apply.Find(funds_Apply.number);
                if (apply == null)
                {
                    json.msg_code = "error";
                    json.msg_text = "没有主申请单，更新失败。";
                    goto next;
                }
                if (apply.apply_state != 1)
                {
                    apply.apply_amount = funds_Apply.amount;
                    apply.apply_state = 0;
                    apply.apply_time = DateTime.Now;
                    db.Entry(apply).State = EntityState.Modified;
                    if (funds_Apply.next == 0)
                    {
                        json.msg_code = "error";
                        json.msg_text = "必需选择批复人。";
                        goto next;
                    }
                    //删除原子申请单及批复
                    var clist = (from c in db.Funds_Apply_Child where c.c_apply_number == apply.apply_number select c).ToList();
                    foreach (var item in clist)
                    {
                        var plist = (from p in db.Process_Respond where p.pr_apply_number == item.c_child_number select p).ToList();
                        if (plist != null)
                        {
                            db.Process_Respond.RemoveRange(plist);
                        }
                    }
                    db.Funds_Apply_Child.RemoveRange(clist);
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
                    foreach (ApplyChildModel citem in funds_Apply.child)
                    {
                        var funds = (from fs in db.Funds
                                     where fs.f_id == citem.Fid
                                     select fs).FirstOrDefault();
                        if (funds.f_amount == 0)
                        {
                            json.msg_code = "error";
                            json.msg_text = string.Format("申请单提交失败,id为{0}的经费没有设置总额。", citem.Fid);
                            goto next;
                        }
                        if (funds.f_balance < citem.amount)
                        {
                            json.msg_code = "error";
                            json.msg_text = string.Format("申请单提交失败,id为{0}的经费不足。", citem.Fid);
                            goto next;
                        }
                        Reimbursement_Content capply = new Reimbursement_Content();
                        capply.c_apply_number = apply.apply_number;
                        capply.c_child_number = string.Format("{0}-{1}", apply.apply_number, GetIntStr(i));
                        capply.c_amount = citem.amount;
                        capply.c_get = citem.amount;
                        capply.c_get_info = "";
                        capply.c_apply_for = citem.applyFor;
                        capply.c_funds_id = citem.Fid;
                        capply.c_state = 0;
                        db.Funds_Apply_Child.Add(capply);
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
                        Process_Respond pr = new Process_Respond();
                        pr.pr_apply_number = capply.c_child_number;
                        pr.pr_user_id = funds_Apply.next;
                        pr.pr_number = 1;
                        db.Process_Respond.Add(pr);
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
                            catch (Exception et) { }
                            json.msg_code = "error";
                            json.msg_text = "申请单批复流程生成失败。";
                            goto next;
                        }
                        i++;
                    }
                }
                else
                {
                    //如果实领不等于申请，需要重调余额
                    foreach (ApplyChildModel citem in funds_Apply.child)
                    {
                        if (string.IsNullOrEmpty(citem.Cnumber)) continue;
                        Reimbursement_Content fac = db.Funds_Apply_Child.Find(citem.Cnumber);
                        if (fac == null) continue;
                        if (citem.factGet != null && citem.factGet != 0)
                        {
                            fac.c_get = (decimal)citem.factGet;
                            fac.c_get_info = citem.getInfo;
                            db.Entry(fac).State = EntityState.Modified;
                            if (fac.c_get != fac.c_amount)
                            {
                                Funds funds = db.Funds.Find(fac.c_funds_id);
                                funds.f_balance = funds.f_balance + (fac.c_amount - fac.c_get);
                                db.Entry(funds).State = EntityState.Modified;
                            }
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                json.msg_code = "error";
                                json.msg_text = "修改失败。";
                                goto next;
                            }
                        }
                    }
                    
                }
                json.state = 1;
                json.msg_code = "success";
                json.msg_text = apply.apply_number;
            }
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="cnumber"></param>
        /// <returns></returns>
        [HttpPost]
        // GET: ApplyManager/Delete/5
        public JsonResult Delete(string number)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_code = "nologin";
                goto next;
            }
            if (number == null)
            {
                json.msg_code = "errorNumber";
                json.msg_text = "申请单号获取失败。";
                goto next;
            }
            //查询订单状态，如果已批复，不能撤销。如果没有，删除流程。
            Reimbursement fundsApply = db.Funds_Apply.Find(number);
            if (fundsApply == null)
            {
                json.msg_code = "nodate";
                json.msg_text = "申请单不存在或被删除。";
                goto next;
            }
            if (fundsApply.apply_state == 1)
            {
                json.msg_code = "forbidden";
                json.msg_text = "已批复同意的申请单不允许删除。";
                goto next;
            }
            var cs = db.Funds_Apply_Child.Where(x => x.c_apply_number == fundsApply.apply_number);
            if (cs.Count() > 0)
                foreach (Reimbursement_Content citem in cs)
                {
                    var prs = db.Process_Respond.Where(x => x.pr_apply_number == citem.c_child_number);
                    if(prs.Count()>0)
                    foreach (Process_Respond pr in prs) db.Process_Respond.Remove(pr);
                    db.Funds_Apply_Child.Remove(citem);
                }
            db.Funds_Apply.Remove(fundsApply);
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
                            join das in db.Dic_Respond_State
                            on apply.apply_state equals das.drs_state_id
                            where apply.apply_user_id == user// && apply.apply_state == 3
                            select new ApplyListModel
                            {
                                amount = apply.apply_amount,
                                number = apply.apply_number,
                                state = apply.apply_state,
                                strState = das.drs_state_name,
                                time = apply.apply_time,
                                child = (from c in db.Funds_Apply_Child
                                         join das in db.Dic_Respond_State on c.c_state equals das.drs_state_id
                                         join f in db.Funds on c.c_funds_id equals f.f_id
                                         where c.c_apply_number == apply.apply_number
                                         select new ApplyChildModel
                                         {
                                             Cnumber = c.c_child_number,
                                             fundsCode = f.f_code,
                                             amount = c.c_amount,
                                             state=c.c_state,
                                             strState = das.drs_state_name,
                                             factGet = c.c_get
                                         }
                                              ).ToList()
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
            Reimbursement funds_Apply = db.Funds_Apply.Where(x => x.apply_number == number).FirstOrDefault();
            if (funds_Apply == null)
            {
                return HttpNotFound();
            }
            var cmList = (from child in db.Funds_Apply_Child
                          join funds in db.Funds
                          on child.c_funds_id equals funds.f_id
                          join das in db.Dic_Respond_State on child.c_state equals das.drs_state_id
                          where child.c_apply_number == number
                          select new ApplyFundsManager
                          {
                              Cnumber = child.c_child_number,
                              strState = das.drs_state_name
                          }
                          ).ToList();
            if (cmList.Count() > 0) ViewBag.number = number;
            foreach (ApplyFundsManager item in cmList)
            {
                var plist = (from p in db.Process_Respond
                             join s in db.Dic_Respond_State
                             on p.pr_state equals s.drs_state_id into T1
                             from t1 in T1.DefaultIfEmpty()
                             join u in db.User_Info
                             on p.pr_user_id equals u.user_id into T2
                             from t2 in T2.DefaultIfEmpty()
                             where p.pr_apply_number == item.Cnumber
                             select new ListResponseModel
                             {
                                 capply_number = p.pr_apply_number,
                                 content = p.pr_content,
                                 id = p.pr_id,
                                 number = p.pr_number,
                                 strState = t1.drs_state_name,
                                 state = p.pr_state,
                                 time = p.pr_time,
                                 user = t2.user_name
                             }
                             ).ToList();
                item.processList = plist;
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
