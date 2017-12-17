using FundsManager.Common;
using FundsManager.DAL;
using FundsManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FundsManager.Controllers
{
    public static class DBCaches<T>
    {
        private static string cache_post = "cache_post";
        private static FundsContext db = new FundsContext();
        public static List<Dic_Post> getPost()
        {
            List<Dic_Post> list = (List<Dic_Post>)DataCache.GetCache(cache_post);
            if (list == null)
            {
                list = new List<Dic_Post>();
                list = (from ts in db.Dic_Post
                           select new Dic_Post
                           {
                               post_id=ts.post_id,
                               post_name=ts.post_name
                           }).ToList();
                DataCache.SetCache(cache_post, list,DateTime.Now.AddYears(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return list;
        }
        public static List<Dic_Role> getRole()
        {
            List<Dic_Role> list = (List<Dic_Role>)DataCache.GetCache(cache_post);
            if (list == null)
            {
                list = (from ts in db.Dic_Role
                           select new Dic_Role
                           {
                               role_id=ts.role_id,
                               role_name=ts.role_name
                           }).ToList();
                DataCache.SetCache(cache_post, list, DateTime.Now.AddYears(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return list;
        }
        public static List<T> getCache(string cache_name)
        {
            List<T> list = (List<T>)DataCache.GetCache(cache_name);
            if (list == null)
            {
                list = db.Database.SqlQuery<T>(string.Format("select * from {0}",typeof(T).ToString())).ToList();

                DataCache.SetCache(cache_post, list, DateTime.Now.AddYears(1), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return list;
        }

        public static void ClearCache(string cache_name)
        {
            DataCache.RemoveCache(cache_name);
        }
        public static void ClearAllCache()
        {
            DataCache.RemoveAllCache();
        }
    }
}