﻿using System.Collections.Generic;
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
        private static string funds_manger = "funds_manger";
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
            List<SelectOption> options = (List<SelectOption>)DataCache.GetCache(funds_manger);
            if (options == null)
            {
                options = (from user in db.User_Info
                           join uvr in db.User_vs_Role
                           on user.user_id equals uvr.uvr_user_id into T1
                           from t1 in T1.DefaultIfEmpty()
                           where t1.uvr_role_id == 1 || t1.uvr_role_id == 2
                           select new SelectOption
                           {
                               id = user.user_id.ToString(),
                               text = user.real_name
                           }).ToList();
                if (options.Count() > 0) DataCache.SetCache(funds_manger, options);
            }
            return options;
        }
    }
}