using FundsManager.DAL;
using FundsManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using FundsManager.Common;
namespace FundsManager.BLL
{
    public static class Dic_Role
    {
        private static FundsContext db = new FundsContext();
        private static string cache_key = "dic-role";
        public static List<RoleInfo> getRoleInfo()
        {
            object roles = DataCache.GetCache(cache_key);
            if (roles == null)
            {
                var list = (from role in db.Dic_Role
                            select new RoleInfo
                            {
                                id = role.role_id,
                                name = role.role_name
                            }).ToList();
                if (list.Count() > 0) DataCache.SetCache(cache_key, list, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration);
                return list;
            }
            else return (List<RoleInfo>)roles;
            
        }
    }
}