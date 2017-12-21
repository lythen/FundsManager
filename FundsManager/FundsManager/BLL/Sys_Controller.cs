using FundsManager.Common;
using FundsManager.DAL;
using FundsManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FundsManager.BLL
{
    public class Sys_Controller
    {
        private static FundsContext db = new FundsContext();
        private static string cache_key = "dic-module";
        public static List<ModuleInfo> getRoleInfo()
        {
            object roles = DataCache.GetCache(cache_key);
            if (roles == null)
            {
                var list = (from mod in db.Sys_Controller
                            select new ModuleInfo
                            {
                                id = mod.id,
                                name = mod.controller_name,
                                info=mod.controller_info
                            }).ToList();
                if (list.Count() > 0) DataCache.SetCache(cache_key, list, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
                return list;
            }
            else return (List<ModuleInfo>)roles;

        }
    }
}