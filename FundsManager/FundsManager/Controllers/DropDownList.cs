using System.Collections.Generic;
using System.Web.Mvc;
using FundsManager.DAL;
using FundsManager.Models;
using System.Linq;
using FundsManager.Common;

namespace FundsManager.Controllers
{
    public static class DropDownList
    {
        private static FundsContext db = new FundsContext();
        private static string cache_week = "cache_week";
        private static string cache_post = "cache_post";
        private static string cache_funds_manger = "funds_manger";
        private static string cache_dept = "cache_dept";
        private static string cache_sex = "cache_sex";
        private static string cache_user_state = "cache_user_state";
        private static string cache_role = "cache_role";
        private static string cache_cardType = "cache_cardType";
        private static string cache_funds = "cache_funds";
        public static List<SelectListItem> SetDropDownList(List<Models.SelectOption> options)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (options != null)
            {
                foreach (Models.SelectOption item in options)
                {
                    items.Add(new SelectListItem { Text = item.text, Value = item.id });
                }
            }
            return items;
        }
        public static List<SelectOption> WeekSelect()
        {
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(cache_week);
            if (options == null)
            {
                options = new List<SelectOption>();
                options.Add(new SelectOption { text = "日", id = "0" });
                options.Add(new SelectOption { text = "一", id = "1" });
                options.Add(new SelectOption { text = "二", id = "2" });
                options.Add(new SelectOption { text = "三", id = "3" });
                options.Add(new SelectOption { text = "四", id = "4" });
                options.Add(new SelectOption { text = "五", id = "5" });
                options.Add(new SelectOption { text = "六", id = "6" });
                DataCache.SetCache(cache_week, options);
            }
            return options;
        }
        public static List<SelectOption> PostSelect()
        {
            List<SelectOption> options = new List<SelectOption>();
            var post = DBCaches<Dic_Post>.getCache(cache_post);
            if (post != null)
            {
                foreach (var item in post)
                {
                    options.Add(new SelectOption { text = item.post_name, id = item.post_id.ToString() });
                }
            }
            return options;
        }
        public static List<SelectOption> FundsManagerSelect()
        {
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(cache_funds_manger);
            if (options == null)
            {
                options = (from op in (from user in db.User_Info
                                       join uvr in db.User_vs_Role
                                       on user.user_id equals uvr.uvr_user_id into T1
                                       from t1 in T1.DefaultIfEmpty()
                                       where t1.uvr_role_id == 1 || t1.uvr_role_id == 2
                                       select new
                                       {
                                           id = user.user_id,
                                           text = user.real_name
                                       }).ToList()
                           select new SelectOption
                           {
                               id = op.id.ToString(),
                               text = Common.DEncrypt.AESEncrypt.Decrypt(op.text)
                           }).ToList();
                if (options.Count() > 0) DataCache.SetCache(cache_funds_manger, options);
            }
            return options;
        }
        public static List<SelectOption> getDepartment(int id=0)
        {
            List<Dic_Department> depts = DBCaches<Dic_Department>.getCache(cache_dept);
            List<SelectOption> option = (from dep in depts
                                         where dep.dept_parent_id == id
                                         select new SelectOption
                                         {
                                             id = dep.dept_id.ToString(),
                                             text = dep.dept_name
                                         }).ToList();
            return option;
        }
        public static List<SelectOption> SexSelect()
        {
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(cache_sex);
            if (options == null)
            {
                options = new List<SelectOption>();
                options.Add(new SelectOption { text = "男", id = "男" });
                options.Add(new SelectOption { text = "女", id = "女" });
                DataCache.SetCache(cache_sex, options);
            }
            return options;
        }
        public static List<SelectOption> UserStateSelect()
        {
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(cache_user_state);
            if (options == null)
            {
                options = new List<SelectOption>();
                options.Add(new SelectOption { text = "正常", id = "1" });
                options.Add(new SelectOption { text = "未启用", id = "0" });
                options.Add(new SelectOption { text = "锁定", id = "2" });
                DataCache.SetCache(cache_user_state, options);
            }
            return options;
        }
        public static List<SelectOption> CardTypeSelect()
        {
            List<Dic_CardType> depts = DBCaches<Dic_CardType>.getCache(cache_cardType);
            List<SelectOption> option = (from ct in depts
                                         select new SelectOption
                                         {
                                             id = ct.ctype_name,
                                             text = ct.ctype_name
                                         }).ToList();
            return option;
        }
        public static List<SelectOption> RoleSelect()
        {
            List<Dic_Role> depts = DBCaches<Dic_Role>.getCache(cache_role);
            List<SelectOption> option = (from ct in depts
                                         select new SelectOption
                                         {
                                             id = ct.role_id.ToString(),
                                             text = ct.role_name
                                         }).ToList();
            return option;
        }
        public static List<SelectOption> FundsSelect()
        {
            List<Funds> funds = DBCaches<Funds>.getCache(cache_dept);
            List<SelectOption> option = (from fund in funds
                                         where fund.f_state==1
                                         select new SelectOption
                                         {
                                             id = fund.f_id.ToString(),
                                             text = fund.f_name
                                         }).ToList();
            return option;
        }
    }
}