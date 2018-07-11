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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cmList = (from bill in db.Reimbursement
                          join respond in db.Process_Respond on bill.reimbursement_code equals respond.pr_reimbursement_code
                          join u in db.User_Info on respond.pr_user_id equals u.user_id
                          join r in db.Dic_Respond_State on respond.pr_state equals r.drs_state_id
                          where bill.reimbursement_code == id
                          select new ApplyFundsManager
                          {
                              reimbursementCode = bill.reimbursement_code,
                              strManager = u.real_name,
                              strState = r.drs_state_name
                          }
                          ).FirstOrDefault();
            ViewBag.number = id;
            var plist = (from p in db.Process_Respond
                         join s in db.Dic_Respond_State
                         on p.pr_state equals s.drs_state_id into T1
                         from t1 in T1.DefaultIfEmpty()
                         join u in db.User_Info
                         on p.pr_user_id equals u.user_id into T2
                         from t2 in T2.DefaultIfEmpty()
                         where p.pr_reimbursement_code == cmList.reimbursementCode
                         select new ListResponseModel
                         {
                             reimbursementCode = p.pr_reimbursement_code,
                             content = p.pr_content,
                             id = p.pr_id,
                             number = p.pr_number,
                             strState = t1.drs_state_name,
                             state = p.pr_state,
                             time = p.pr_time,
                             user = t2.user_name
                         }
                         ).ToList();
            cmList.processList = plist;
            
            return View(cmList);
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
                if (funds.f_manager == user)
                {
                    json.msg_code = "forbidden";
                    json.msg_text = "不允许申请自己的经费。";
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
            ApplyListModel viewBill = (from bill in db.Reimbursement
                                       join das in db.Dic_Respond_State
                                       on bill.r_bill_state equals das.drs_state_id
                                       where bill.reimbursement_code == id && bill.r_add_user_id == user
                                       select new ApplyListModel
                                       {
                                           amount = bill.r_bill_amount,
                                           reimbursementCode = bill.reimbursement_code,
                                           state = bill.r_bill_state,
                                           strState = das.drs_state_name,
                                           time = bill.r_add_date,
                                           Fid = bill.r_funds_id,
                                           factAmount = bill.r_fact_amount,

                                           contents = (from c in db.Reimbursement_Content
                                                       where c.c_reimbursement_code == bill.reimbursement_code
                                                       select new ViewContentModel
                                                       {
                                                           reimbursementCode = c.c_reimbursement_code,
                                                           amount = c.c_amount,
                                                           contentId = c.content_id,
                                                           details = (from detail in db.Reimbursement_Detail
                                                                      where detail.detail_content_id == c.content_id
                                                                      select new ViewDetailContent
                                                                      {
                                                                          amount = detail.detail_amount,
                                                                          contentId = detail.detail_content_id,
                                                                          detailDate = detail.detail_date,
                                                                          detailId = detail.detail_id,
                                                                          detailInfo = detail.detail_info
                                                                      }
                                                                      ).ToList()
                                                       }
                                                         ).ToList(),
                                           attachments = (from attachment in db.Reimbursement_Attachment
                                                          where attachment.atta_reimbursement_code == bill.reimbursement_code
                                                          select new ViewAttachment {
                                                           fileName=attachment.attachment_path,
                                                           id=attachment.attachment_id,
                                                           reimbursementCode=attachment.atta_reimbursement_code}).ToList()
                                       }
                            ).FirstOrDefault();
            return View(viewBill);
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
                        }
                        if(content==null)
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
                                if (detail == null)
                                {
                                    detail.detail_content_id = content.content_id;
                                }
                                detail.detail_amount = item.amount;
                                detail.detail_date = DateTime.Parse(item.strDate+" 00:00");
                                detail.detail_info = item.detailInfo;
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
                                sbmsg.Append("报销明细录入失败<br />");
                                continue;
                            }
                        }
                    }
                    //录入附件
                    if (viewBill.attachments != null && viewBill.attachments.Count() > 0)
                    {
                        string attachment_path = ConfigurationManager.AppSettings["attachmentPath"];
                        string attachment_temp_path = ConfigurationManager.AppSettings["attachmentTempPath"];
                        if (!Directory.Exists(attachment_path)) Directory.CreateDirectory(attachment_path);
                        string filePath, tempFile, saveFileName;
                        foreach (ViewAttachment item in viewBill.attachments)
                        {
                            if (item.id > 0) continue;
                            try
                            {
                                saveFileName = string.Format("{0}\\{1}\\{2}", bill.reimbursement_code, DateTime.Now.ToString("yyyyMMdd"), item.fileName);
                                tempFile = attachment_temp_path + item.fileName;
                                filePath = string.Format("{0}{1}", attachment_path, saveFileName);
                                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                                System.IO.File.Move(tempFile, filePath);
                            }
                            catch
                            {
                                sbmsg.Append("文件【").Append(item.fileName).Append("】保存失败，请重新上传");
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
                        try
                        {
                            //干脆都先提交得了
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            Delete(bill.reimbursement_code);
                            json.msg_code = "error";
                            json.msg_text = "报销单附件提交失败。";
                            goto next;
                        }
                    }
                    //录入批复流程
                    db.Process_Respond.RemoveRange(db.Process_Respond.Where(x => x.pr_reimbursement_code == bill.reimbursement_code));
                    Process_Respond pr = new Process_Respond();
                    pr.pr_reimbursement_code = bill.reimbursement_code;
                    pr.pr_user_id = viewBill.next;
                    pr.pr_number = 1;
                    db.Process_Respond.Add(pr);
                }
                json.state = 1;
                json.msg_code = bill.reimbursement_code;
                json.msg_text = sbmsg.ToString();
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
            throw new Exception();
        }
        public JsonResult DeleteContentDetail(int id)
        {
            throw new Exception();
        }
        public JsonResult DeleteAttachment(int id)
        {
            throw new Exception();
        }
        public ActionResult MyFunds(BillsSearchModel info)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            ApplyManager dal = new ApplyManager();
            if (!RoleCheck.CheckIsAdmin(user)) info.userId = user;
            info.PageSize = 0;
            var list = dal.GetApplyList(info);
            var waitList = (from bill in list
                            select new ApplyListModel
                            {
                                amount = bill.amount,
                                reimbursementCode = bill.reimbursementCode,
                                state = bill.state,
                                strState = bill.strState,
                                time = bill.time,
                                 fundsCode= bill.fundsCode,
                                 fundsName= bill.fundsName,
                                attachmentsCount = (from a in db.Reimbursement_Attachment where a.atta_reimbursement_code == bill.reimbursementCode select a.attachment_id).Count(),
                                contents = (from c in db.Reimbursement_Content
                                            join Dic in db.Dic_Reimbursement_Content on c.c_dic_id equals Dic.content_id
                                            where c.c_reimbursement_code == bill.reimbursementCode
                                            select new ViewContentModel
                                            {
                                                amount = c.c_amount,
                                                contentId = c.content_id,
                                                reimbursementCode = c.c_reimbursement_code,
                                                contentTitle = Dic.content_title

                                            }
                                              ).ToList()
                            }
                            ).ToList();
            ViewData["Bills"] = waitList;
            return View(info);
        }
        public ActionResult ApplyNext(string number)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = Common.PageValidate.FilterParam(User.Identity.Name);
            if (number == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cmList = (from bill in db.Reimbursement
                          join das in db.Dic_Respond_State
                          on bill.r_bill_state equals das.drs_state_id
                          join f in db.Funds on bill.r_funds_id equals f.f_id
                          where bill.r_add_user_id == user// && apply.apply_state == 3
                          select new ApplyListModel
                          {
                              amount = bill.r_bill_amount,
                              reimbursementCode = bill.reimbursement_code,
                              state = bill.r_bill_state,
                              strState = das.drs_state_name,
                              time = bill.r_add_date,
                              fundsCode = f.f_code,
                              fundsName = f.f_name,
                              attachmentsCount = (from a in db.Reimbursement_Attachment where a.atta_reimbursement_code == bill.reimbursement_code select a.attachment_id).DefaultIfEmpty(0).Count(),
                              contents = (from c in db.Reimbursement_Content
                                          join Dic in db.Dic_Reimbursement_Content on c.c_dic_id equals Dic.content_id
                                          where c.c_reimbursement_code == bill.reimbursement_code
                                          select new ViewContentModel
                                          {
                                              amount = c.c_amount,
                                              contentId = c.content_id,
                                              reimbursementCode = c.c_reimbursement_code,
                                              contentTitle = Dic.content_title

                                          }
                                            ).ToList()
                          }
                            ).ToList();
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
