using System.Collections.Generic;
using System.Web.Mvc;
using FundsManager.DAL;
using FundsManager.Models;
using FundsManager.ViewModels;
using FundsManager.Common;
using System.Linq;
using System;
using FundsManager.Common.DEncrypt;

namespace FundsManager.Controllers
{
    public class StatisticsController : Controller
    {
        private FundsContext db = new FundsContext();
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
            var query = dal.GetReimbursement("", (int)search.userId);
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
                item.attachmentsCount= dal.getAttachments(item.reimbursementCode, 0).Count();
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