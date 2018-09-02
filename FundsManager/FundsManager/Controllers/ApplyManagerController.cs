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
using System.Data.Entity.Validation;
using System.Text;
using System.IO;
using FundsManager.Common.DEncrypt;

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
            //var waitList = (from bill in db.Reimbursement
            //                join s in db.Dic_Respond_State on bill.r_bill_state equals s.drs_state_id
            //                where bill.r_add_user_id == user && bill.r_bill_state != 3
            //                select new ApplyListModel
            //                {
            //                    amount = bill.r_bill_amount,
            //                    number = bill.reimbursement_code,
            //                    state = bill.r_bill_state,
            //                    time = bill.r_add_time,
            //                    strState = s.drs_state_name,
            //                    child = (from content in db.Reimbursement_Content
            //                             where child.c_apply_number == bill.apply_number
            //                             select new ApplyChildModel
            //                             {
            //                                 Cnumber = child.c_child_number,
            //                                 fundsCode = f.f_code,
            //                                 amount = child.c_amount,
            //                                 strState = das.drs_state_name,
            //                                 factGet = child.c_get
            //                             }
            //                                  ).ToList()
            //                }
            //                ).ToList();
            return View();
        }

        // GET: ApplyManager/Details/5
        public ActionResult Details(string id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            if (id == null)
            {
                return RedirectToRoute(new { controller = "Error", action = "Index", err = "报销单号获取失败。" });
            }
            ApplyManager dal = new ApplyManager(db);
            List<SelectOption> options = DropDownList.RespondUserSelect();
            ViewData["ViewUsers"] = DropDownList.SetDropDownList(options);
            var bill = dal.GetReimbursement(id,0).FirstOrDefault();
            bill.contents = dal.getContents(bill.reimbursementCode, 0).ToList();
            foreach(var item in bill.contents)
            {
                item.details = dal.getContentDetails((int)item.contentId, 0).ToList();
            }
            bill.attachments = dal.getAttachments(bill.reimbursementCode, 0).ToList();
            bill.responds = dal.getResponds(bill.reimbursementCode, 0).ToList();

            bill.userName= AESEncrypt.Decrypt(bill.userName);
            if (bill.responds != null && bill.responds.Count() > 0)
            {
                foreach (var respond in bill.responds)
                    respond.respondUser = AESEncrypt.Decrypt(respond.respondUser);
            }
            return View(bill);
        }
        // GET: ApplyManager/Create
        public ActionResult Create()
        {
            //if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            //int user = Common.PageValidate.FilterParam(User.Identity.Name);
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
                //if (funds.f_manager == user)
                //{
                //    json.msg_code = "forbidden";
                //    json.msg_text = "不允许申请自己的经费。";
                //    goto next;
                //}
                if (funds.f_balance < _sbill.amount)
                {
                    json.msg_code = "error";
                    json.msg_text = string.Format("报销单提交失败,id为{0}的经费不足。", _sbill.Fid);
                    goto next;
                }

                Reimbursement bill = new Reimbursement();
                bill.r_bill_amount = _sbill.amount;
                bill.r_bill_state = 0;
                bill.r_add_date = DateTime.Now;
                bill.r_add_user_id = user;
                bill.reimbursement_info = _sbill.info;
                bill.r_funds_id = _sbill.Fid;
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
                    ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().ToString());
                    json.msg_code = "error";
                    json.msg_text = "报销单提交失败。";
                    goto next;
                }
                //添加报销内容
                foreach (ViewContentModel citem in _sbill.contents)
                {
                    Reimbursement_Content content = new Reimbursement_Content();
                    content.c_reimbursement_code = bill.reimbursement_code;
                    content.c_amount = citem.amount;
                    content.c_dic_id = citem.selectId;
                    db.Reimbursement_Content.Add(content);
                    try
                    {
                        //必需先提交更改，因为下面添加明细需要用到自动生成的ID。
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().ToString());
                        Delete(bill.reimbursement_code);
                        json.msg_code = "error";
                        json.msg_text = "报销单提交失败。";
                        goto next;
                    }
                    //添加明细
                    if (citem.details!=null && citem.details.Count() > 0)
                    {
                        foreach(ViewDetailContent viewDetail in citem.details)
                        {
                            Reimbursement_Detail detail = new Reimbursement_Detail()
                            {
                                detail_amount = viewDetail.amount,
                                detail_content_id = content.content_id,
                                detail_date = DateTime.Parse(viewDetail.strDate+" 00:00"),
                                detail_info = viewDetail.detailInfo
                            };
                            db.Reimbursement_Detail.Add(detail);
                        }
                        try
                        {
                            //干脆都先提交得了
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().ToString());
                            Delete(bill.reimbursement_code);
                            json.msg_code = "error";
                            json.msg_text = "报销单提交失败。";
                            goto next;
                        }
                    }
                }

                //添加附件
                StringBuilder sbErr = new StringBuilder();
                if (_sbill.attachments != null && _sbill.attachments.Count() > 0)
                {
                    string attachment_path = string.Format("{0}\\{1}\\{2}\\", MyConfiguration.GetAttachmentPath(), bill.reimbursement_code,DateTime.Now.ToString("yyyyMMdd"));
                    string attachment_temp_path = MyConfiguration.GetAttachmentTempPath(); ;
                    if (!Directory.Exists(attachment_path)) Directory.CreateDirectory(attachment_path);
                    string filePath,tempFile, saveFileName = "",storeFileName;
                    foreach (ViewAttachment item in _sbill.attachments)
                    {
                        try
                        {
                            saveFileName = Path.GetFileName(item.fileName);
                            storeFileName = string.Format("{0}/{1}", DateTime.Now.ToString("yyyyMMdd"), saveFileName);
                            tempFile = attachment_temp_path + item.fileName;
                            filePath = string.Format("{0}{1}", attachment_path, saveFileName);
                            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                            System.IO.File.Move(tempFile, filePath);
                        }
                        catch(Exception e)
                        {
                            ErrorUnit.WriteErrorLog(e.ToString(), GetType().ToString());
                            sbErr.Append("文件【").Append(saveFileName).Append("】保存失败，请重新上传");
                            continue;
                        }
                        Reimbursement_Attachment attachment = new Reimbursement_Attachment
                        {
                            attachment_path = storeFileName,
                            atta_detail_id = 0,
                            atta_reimbursement_code = bill.reimbursement_code
                        };
                        db.Reimbursement_Attachment.Add(attachment);
                    }
                    try
                    {
                        //干脆都先提交得了
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().ToString());
                        Delete(bill.reimbursement_code);
                        json.msg_code = "error";
                        json.msg_text = "报销单提交失败。";
                        goto next;
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
                    SysLog.WriteLog(user, string.Format("添加报帐单[{0}]", bill.reimbursement_code), IpHelper.GetIP(), bill.reimbursement_code, 4, "", db);
                }
                catch (Exception e)
                {
                    ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().ToString());
                    Delete(bill.reimbursement_code);
                    json.msg_code = "error";
                    json.msg_text = "报销单提交失败。";
                    goto next;
                }
                json.state = 1;
                json.msg_code = bill.reimbursement_code;
                json.msg_text = sbErr.ToString(); 
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
            options = DropDownList.RespondUserSelect();
            ViewData["ViewUsers"] = DropDownList.SetDropDownList(options);
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
            ApplyManager dal = new ApplyManager(db);
            ApplyListModel bill = dal.GetReimbursement(id, user).FirstOrDefault();
            bill.contents = dal.getContents(bill.reimbursementCode, 0).ToList();
            foreach (var item in bill.contents)
            {
                item.details = dal.getContentDetails((int)item.contentId, 0).ToList();
            }
            bill.attachments = dal.getAttachments(bill.reimbursementCode, 0).ToList();
            var responds = dal.getResponds(bill.reimbursementCode, 0).OrderBy(x => x.num).FirstOrDefault();
            if (responds != null) bill.next = (int)responds.thisRespondUser;
            return View(bill);
        }

        // POST: ApplyManager/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public JsonResult Edit(ApplyListModel viewBill)
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
                Reimbursement bill = db.Reimbursement.Find(viewBill.reimbursementCode);
                if (bill == null)
                {
                    json.msg_code = "error";
                    json.msg_text = "没有主申请单，更新失败。";
                    goto next;
                }
                if (viewBill.next == 0)
                {
                    json.msg_code = "error";
                    json.msg_text = "必需选择批复人。";
                    goto next;
                }
                var funds = (from fs in db.Funds
                             where fs.f_id == viewBill.Fid
                             select fs).FirstOrDefault();
                if (funds.f_amount == 0)
                {
                    json.msg_code = "error";
                    json.msg_text = string.Format("报销单提交失败,经费{0}没有设置总额。", funds.f_code);
                    goto next;
                }
                if (funds.f_balance < viewBill.amount)
                {
                    json.msg_code = "error";
                    json.msg_text = string.Format("申请单提交失败,经费{0}不足。", funds.f_code);
                    goto next;
                }
                StringBuilder sbmsg = new StringBuilder();
                if (bill.r_bill_state == 1)
                {
                    json.msg_code = "error";
                    json.msg_text = "该报销单已经批复完成，不允许修改。";
                    goto next;
                }
                else
                {
                    bill.r_bill_amount = viewBill.amount;
                    bill.r_fact_amount = viewBill.amount;
                    bill.r_bill_state = 0;
                    bill.r_add_date = DateTime.Now;
                    bill.r_funds_id = viewBill.Fid;
                    bill.reimbursement_info = viewBill.info;
                    db.Entry(bill).State = EntityState.Modified;
                    //录入报销事由
                    Reimbursement_Content content=null;
                   
                    foreach (ViewContentModel citem in viewBill.contents)
                    {
                        if (citem.contentId != null && citem.contentId != 0)
                        {
                            content = db.Reimbursement_Content.Find(citem.contentId);
                            if (content != null)
                            {
                                content.c_amount = citem.amount;
                                db.Entry(content).State = EntityState.Modified;
                            }
                        }else
                        {
                            content = new Reimbursement_Content();
                            content.c_reimbursement_code = bill.reimbursement_code;
                            content.c_dic_id = citem.selectId;
                            content.c_amount = citem.amount;
                            db.Reimbursement_Content.Add(content);
                        }
                        
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().ToString());
                            sbmsg.Append("报销内容录入失败<br />");
                            continue;
                        }

                        //录入明细
                        if (citem.details != null && citem.details.Count() > 0)
                        {
                            Reimbursement_Detail detail=null;
                            foreach(ViewDetailContent item in citem.details)
                            {
                                if (item.detailId != null && item.detailId != 0)
                                    detail = db.Reimbursement_Detail.Find(item.detailId);
                                else
                                {
                                    detail = new Reimbursement_Detail();
                                    detail.detail_content_id = content.content_id;
                                }
                                detail.detail_amount = item.amount;
                                detail.detail_date = DateTime.Parse(item.strDate+" 00:00");
                                detail.detail_info = item.detailInfo;
                                detail.detail_content_id = content.content_id;
                                if (item.detailId != null && item.detailId != 0)
                                    db.Entry(detail).State = EntityState.Modified;
                                else db.Reimbursement_Detail.Add(detail);
                            }
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().ToString());
                                sbmsg.Append("报销明细录入失败<br />");
                                continue;
                            }
                        }
                    }
                    //录入附件
                    if (viewBill.attachments != null && viewBill.attachments.Count() > 0)
                    {
                        string attachment_path = string.Format("{0}\\{1}\\{2}\\", MyConfiguration.GetAttachmentPath(), bill.reimbursement_code, DateTime.Now.ToString("yyyyMMdd"));
                        string attachment_temp_path = MyConfiguration.GetAttachmentTempPath(); ;
                        if (!Directory.Exists(attachment_path)) Directory.CreateDirectory(attachment_path);
                        string filePath, tempFile, saveFileName = "", storeFileName;
                        foreach (ViewAttachment item in viewBill.attachments)
                        {
                            if (item.id > 0) continue;
                            try
                            {
                                saveFileName = Path.GetFileName(item.fileName);
                                storeFileName = string.Format("{0}/{1}", DateTime.Now.ToString("yyyyMMdd"), saveFileName);
                                tempFile = attachment_temp_path + item.fileName;
                                filePath = string.Format("{0}{1}", attachment_path, saveFileName);
                                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                                System.IO.File.Move(tempFile, filePath);
                            }
                            catch(Exception e)
                            {
                                ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().ToString());
                                sbmsg.Append("文件【").Append(item.fileName).Append("】保存失败，请重新上传");
                                continue;
                            }
                            Reimbursement_Attachment attachment = new Reimbursement_Attachment
                            {
                                attachment_path = storeFileName,
                                atta_detail_id = 0,
                                atta_reimbursement_code = bill.reimbursement_code
                            };
                            db.Reimbursement_Attachment.Add(attachment);
                        }
                        try
                        {
                            //干脆都先提交得了
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().ToString());
                            Delete(bill.reimbursement_code);
                            json.msg_code = "error";
                            json.msg_text = "报销单附件提交失败。";
                            goto next;
                        }
                    }
                    //录入批复流程
                    db.Process_Respond.RemoveRange(db.Process_Respond.Where(x => x.pr_reimbursement_code == bill.reimbursement_code));
                    //添加批复人
                    Process_Respond pr = new Process_Respond();
                    pr.pr_reimbursement_code = bill.reimbursement_code;
                    pr.pr_user_id = viewBill.next;
                    pr.pr_number = 1;
                    db.Process_Respond.Add(pr);
                    try
                    {
                        db.SaveChanges();
                        SysLog.WriteLog(user, string.Format("修改报帐单[{0}]", bill.reimbursement_code), IpHelper.GetIP(), bill.reimbursement_code, 4, "", db);
                    }
                    catch (Exception e)
                    {
                        ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().ToString());
                        Delete(bill.reimbursement_code);
                        json.msg_code = "error";
                        json.msg_text = "报销单提交失败。";
                        goto next;
                    }
                }
                json.state = 1;
                json.msg_code = bill.reimbursement_code;
                json.msg_text = sbmsg.ToString();
            }
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #region 删除操作
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
                json.msg_text = "报销单号获取失败。";
                goto next;
            }
            //查询订单状态，如果已批复，不能撤销。如果没有，删除流程。
            Reimbursement bill = db.Reimbursement.Find(number);
            if (bill == null)
            {
                json.msg_code = "nodate";
                json.msg_text = "报销单不存在或被删除。";
                goto next;
            }
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            if (user != bill.r_add_user_id)
            {
                json.msg_code = "forbidden";
                json.msg_text = "没有权限操作他人申请的报销单。";
                goto next;
            }
            if (bill.r_bill_state == 1)
            {
                json.msg_code = "forbidden";
                json.msg_text = "已批复同意的报销单不允许删除。";
                goto next;
            }
            var cs = db.Reimbursement_Content.Where(x => x.c_reimbursement_code == bill.reimbursement_code);
            //删除报销内容
            if (cs.Count() > 0)
                foreach (Reimbursement_Content citem in cs)
                {
                    //删除报销细节
                    db.Reimbursement_Detail.RemoveRange(db.Reimbursement_Detail.Where(x => x.detail_content_id == citem.content_id));
                    db.Reimbursement_Content.Remove(citem);
                }
            //删除附件
            db.Reimbursement_Attachment.RemoveRange(db.Reimbursement_Attachment.Where(x => x.atta_reimbursement_code == bill.reimbursement_code));
            //删除批复
            db.Process_Respond.RemoveRange(db.Process_Respond.Where(x => x.pr_reimbursement_code == bill.reimbursement_code));
            //删除总单
            db.Reimbursement.Remove(bill);
            try
            {
                db.SaveChanges();
                SysLog.WriteLog(user, string.Format("删除报帐单[{0}]", bill.reimbursement_code), IpHelper.GetIP(), bill.reimbursement_code, 4, "", db);
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
                json.msg_text = "报销单删除失败。";
                goto next;
            }
            json.state = 1;
            json.msg_code = "success";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteContent(int id)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_code = "nologin";
                goto next;
            }
            Reimbursement_Content content = db.Reimbursement_Content.Find(id);
            if (content == null)
            {
                json.msg_code = "nodate";
                json.msg_text = "报销内容不存在或被删除。";
                goto next;
            }
            Reimbursement bill = db.Reimbursement.Find(content.c_reimbursement_code);
            if (bill != null)
            {
                int user = Common.PageValidate.FilterParam(User.Identity.Name);
                if (user != bill.r_add_user_id)
                {
                    json.msg_code = "forbidden";
                    json.msg_text = "没有权限操作他人申请的报销单。";
                    goto next;
                }
                if (bill.r_bill_state == 1)
                {
                    json.msg_code = "forbidden";
                    json.msg_text = "已批复同意的报销单不允许删除。";
                    goto next;
                }
            }
            var details = db.Reimbursement_Detail.Where(x => x.detail_content_id == content.content_id);
            foreach(var detail in details)
                db.Reimbursement_Detail.Remove(detail);
            db.Reimbursement_Content.Remove(content);
            try
            {
                db.SaveChanges();
            }catch(Exception e)
            {
                ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().Name);
                json.msg_code = "error";
                json.msg_text = "报销单删除失败。";
                goto next;
            }
            json.state = 1;
            json.msg_code = "success";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteContentDetail(int id)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_code = "nologin";
                goto next;
            }
            Reimbursement_Detail detail = db.Reimbursement_Detail.Find(id);
            if (detail == null)
            {
                json.msg_code = "nodate";
                json.msg_text = "报销内容明细不存在或被删除。";
                goto next;
            }
            Reimbursement_Content content = db.Reimbursement_Content.Find(detail.detail_content_id);
            if (content != null)
            {
                Reimbursement bill = db.Reimbursement.Find(content.c_reimbursement_code);
                if (bill != null)
                {
                    int user = Common.PageValidate.FilterParam(User.Identity.Name);
                    if (user != bill.r_add_user_id)
                    {
                        json.msg_code = "forbidden";
                        json.msg_text = "没有权限操作他人申请的报销单。";
                        goto next;
                    }
                    if (bill.r_bill_state == 1)
                    {
                        json.msg_code = "forbidden";
                        json.msg_text = "已批复同意的报销单不允许删除任何信息。";
                        goto next;
                    }
                }
            }
            db.Reimbursement_Detail.Remove(detail);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().Name);
                json.msg_code = "error";
                json.msg_text = "报销内容明细删除失败。";
                goto next;
            }
            json.state = 1;
            json.msg_code = "success";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAttachment(int id)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_code = "nologin";
                goto next;
            }
            Reimbursement_Attachment atta = db.Reimbursement_Attachment.Find(id);
            if (atta == null)
            {
                json.msg_code = "nodate";
                json.msg_text = "附件不存在或被删除。";
                goto next;
            }
            Reimbursement bill = db.Reimbursement.Find(atta.atta_reimbursement_code);
            if (bill != null)
            {
                int user = Common.PageValidate.FilterParam(User.Identity.Name);
                if (user != bill.r_add_user_id&&!RoleCheck.CheckHasAuthority(user,db, "系统管理"))
                {
                    json.msg_code = "forbidden";
                    json.msg_text = "没有权限操作他人申请的报销单。";
                    goto next;
                }
                if (bill.r_bill_state == 1)
                {
                    json.msg_code = "forbidden";
                    json.msg_text = "已批复同意的报销单不允许删除任何信息。";
                    goto next;
                }
            }
            db.Reimbursement_Attachment.Remove(atta);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().Name);
                json.msg_code = "error";
                json.msg_text = "附件删除失败。";
                goto next;
            }
            json.state = 1;
            json.msg_code = "success";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult MyFunds(BillsSearchModel info)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            ApplyManager dal = new ApplyManager(db);
            if (!RoleCheck.CheckHasAuthority(user,db, "经费管理")) info.userId = user;
            info.PageSize = 0;
            info.userId = user;
            var bills = dal.GetApplyList(info).ToList();
            foreach(var bill in bills)
            {
                bill.contents = dal.getContents(bill.reimbursementCode, 0).ToList();
                bill.attachmentsCount = dal.getAttachments(bill.reimbursementCode, 0).Count();
            }
            ViewData["Bills"] = bills;
            List<SelectOption> options = DropDownList.RespondStateSelect();
            ViewData["RState"] = DropDownList.SetDropDownList(options);
            return View(info);
        }
        public JsonResult GetRespondResult(string id)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_code = "nologin";
                goto next;
            }
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            ApplyManager dal = new ApplyManager(db);
            var bill = dal.GetReimbursement(id, 0).FirstOrDefault();
            if (bill == null)
            {
                json.msg_text = "没有找到该报销单。";
                json.msg_code = "noData";
                goto next;
            }
            if (bill.state != 1)
            {
                json.msg_text = "不能打印未批复报销单。";
                json.msg_code = "noData";
                goto next;
            }
            if (user != bill.userId)
            {
                json.msg_text = "不能操作他人报销单。";
                json.msg_code = "noData";
                goto next;
            }
            bill.userName = AESEncrypt.Decrypt(bill.userName);
            bill.contents = dal.getContents(bill.reimbursementCode, 0).ToList();
            bill.attachmentsCount = dal.getAttachments(bill.reimbursementCode, 0).Count();
            bill.detailsCount= (from content in db.Reimbursement_Content
                                join detail in db.Reimbursement_Detail on content.content_id equals detail.detail_content_id
                                where content.c_reimbursement_code == bill.reimbursementCode
                                select content.content_id).Count();

            json.state = 1;
            json.msg_code = "success";
            json.data = bill;
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LogReceive(string id,decimal amount)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_code = "nologin";
                goto next;
            }
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            var bill = db.Reimbursement.Find(id);
            if (bill == null)
            {
                json.msg_text = "没有找到该报销单。";
                json.msg_code = "noData";
                goto next;
            }
            if (bill.r_bill_state != 1)
            {
                json.msg_text = "不能操作未批复报销单。";
                json.msg_code = "noData";
                goto next;
            }
            bool isManager = RoleCheck.CheckHasAuthority(user, db, "系统管理");
            if (!isManager && bill.c_has_log)
            {
                json.msg_text = "已操作录入实际信用金额。";
                json.msg_code = "noData";
                goto next;
            }
            if (user != bill.r_add_user_id&&!isManager)
            {
                json.msg_text = "不能操作他人报销单。";
                json.msg_code = "noData";
                goto next;
            }
            bill.r_fact_amount = amount;
            bill.c_has_log = true;
            db.Entry(bill).State = EntityState.Modified;
            Funds funds = db.Funds.Find(bill.r_funds_id);
            if (funds != null)
            {
                decimal m = bill.r_bill_amount-bill.r_fact_amount;
                funds.f_balance = funds.f_balance + m;
                db.Entry(funds).State = EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
                SysLog.WriteLog(user, string.Format("录入报销单[{0}]实领金额{1}元", bill.reimbursement_code, amount), IpHelper.GetIP(), bill.reimbursement_code, 4, "", db);
            }
            catch (Exception e)
            {
                ErrorUnit.WriteErrorLog(e.ToString(), this.GetType().Name);
                json.msg_code = "error";
                json.msg_text = "录入失败。";
                goto next;
            }
            json.state = 1;
            json.msg_code = "success";
            next:
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
