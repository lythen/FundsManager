using System.Collections.Generic;
using System.Web.Mvc;
using Lythen.DAL;
using Lythen.Models;
using Lythen.ViewModels;
using Lythen.Common;
using System.Linq;
using System;
using Lythen.Common.DEncrypt;

namespace Lythen.Controllers
{
    public class StatisticsController : Controller
    {
        private LythenContext db = new LythenContext();
        public ActionResult Detail(StatisticsSearch search)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            setSearchSelect(user);
            if (!RoleCheck.CheckHasAuthority(user, db, "经费管理"))
            {
                search.manager = user;
                search.userId = user;
            }
            if (search.userId == null) search.userId = 0;
            ApplyManager dal = new ApplyManager(db);
            var query = dal.GetReimbursement("", (int)search.userId).Where(x=>x.state==1);
            if (search.manager > 0) query = query.Where(x => x.manager == search.manager);
            if (search.beginDate != null)
            {
                search.beginDate = DateTime.Parse(((DateTime)search.beginDate).ToString("yyyy-MM-dd 00:00:00.000"));
                query = query.Where(x => x.time >= search.beginDate);
            }
            if (search.endDate != null)
            {
                search.endDate = DateTime.Parse(((DateTime)search.endDate).ToString("yyyy-MM-dd 23:59:59.999"));
                query = query.Where(x => x.time <= search.endDate);
            }
            search.Amount = query.Count();
            query = query.OrderByDescending(x=>x.time).Skip(search.PageSize * (search.PageIndex - 1)).Take(search.PageSize);
            var list = query.ToList();
            foreach(var item in list)
            {
                item.userName = AESEncrypt.Decrypt(item.userName);
                item.attachmentsCount = (from content in db.Reimbursement_Content
                                         join detail in db.Reimbursement_Detail on content.content_id equals detail.detail_content_id
                                         where content.c_reimbursement_code == item.reimbursementCode
                                         select content.content_id).Count();
            }
            ViewData["Details"] = list;
            return View(search);
        }
        public ActionResult FundsStatistics(StatisticsSearch search)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            setSearchSelect(user);
            if (!RoleCheck.CheckHasAuthority(user,db,"统计")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限。" });
            Statistics dal = new Statistics(db);
            if (!RoleCheck.CheckHasAuthority(user, db, "经费管理")) search.manager = user;
            var query = dal.GetFundsStatistics(search);
            ViewData["StatData"] = query;
            return View(search);
        }
        public ActionResult TimesStaticstics(StatisticsSearch search)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToRoute(new { controller = "Login", action = "LogOut" });
            int user = PageValidate.FilterParam(User.Identity.Name);
            setSearchSelect(user);
            if (!RoleCheck.CheckHasAuthority(user, db, "统计")) return RedirectToRoute(new { controller = "Error", action = "Index", err = "没有权限。" });
            Statistics dal = new Statistics(db);
            if (!RoleCheck.CheckHasAuthority(user, db, "经费管理")) search.manager = user;
            var query = dal.GetTimesStatistics(search);
            ViewData["StatData"] = query;
            return View(search);
        }
        public JsonResult GetAllDetail(string id)
        {
            BaseJsonData json = new BaseJsonData();
            if (!User.Identity.IsAuthenticated)
            {
                json.msg_code = "nologin";
                goto next;
            }
            if (id == null)
            {
                json.msg_code = "errorNumber";
                json.msg_text = "报销单号获取失败。";
                goto next;
            }
            //ViewDetailContent
            var query = (from content in db.Reimbursement_Content
                         join detail in db.Reimbursement_Detail on content.content_id equals detail.detail_content_id
                         join dic in db.Dic_Reimbursement_Content on content.c_dic_id equals dic.content_id
                         where content.c_reimbursement_code == id
                         orderby detail.detail_content_id
                         select new ViewDetailContent
                         {
                             contentTitle = dic.content_title,
                             detailInfo = detail.detail_info,
                             amount = detail.detail_amount,
                             detailDate = detail.detail_date
                         }
                        ).ToList();
            json.data = query;
            json.state = 1;
            json.msg_code = "success";
            next:
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        void setSearchSelect(int user)
        {
            List<SelectOption> options = DropDownList.FundsSelect(user);
            ViewData["Funds"] = DropDownList.SetDropDownList(options);
            options = DropDownList.FundsManagerSelect(user);
            ViewData["Managers"] = DropDownList.SetDropDownList(options);
            options = DropDownList.UserSelect(user);
            ViewData["Users"] = DropDownList.SetDropDownList(options);
        }
    }
}