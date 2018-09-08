using Lythen.Common;
using Lythen.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lythen.Controllers
{
    public static class DBCaches<T>
    {
        private static LythenContext db = new LythenContext();
        public static List<T> getCache(string cache_name)
        {
            List<T> list = (List<T>)DataCache.GetCache(cache_name);
            if (list == null)
            {
                list = db.Database.SqlQuery<T>(string.Format("select * from {0}", typeof(T).Name)).ToList();

                DataCache.SetCache(cache_name, list, DateTime.Now.AddYears(1), System.Web.Caching.Cache.NoSlidingExpiration);
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