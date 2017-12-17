using System;
using System.Web;
using System.Collections;
using System.Configuration;
namespace FundsManager.Common
{
    /// <summary>
    /// 缓存相关的操作类，仅适合缓存只读的数据
    /// </summary>
    public class DataCache
    {
        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey">缓存键</param>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey">缓存键</param>
        /// <param name="objObject">缓存对象</param>
        public static void SetCache(string CacheKey, object objObject)
        {
            SetCache(CacheKey, objObject, DateTime.Now.AddSeconds(PageValidate.FilterLongParam(ConfigurationManager.AppSettings["DataTableCache"])), System.Web.Caching.Cache.NoSlidingExpiration);
            //System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            //objCache.Insert(CacheKey, objObject);
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey">缓存键</param>
        /// <param name="objObject">缓存对象</param>
        /// <param name="absoluteExpiration">缓存对象将到期并被从缓存中移除的时间</param>
        /// <param name="slidingExpiration">最后一次访问缓存对象时与该对象到期时之间的时间间隔</param>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// 清除当前应用程序指定CacheKey的Cache值
        /// <param name="CacheKey">缓存键</param>
        /// </summary>
        public static void RemoveCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Remove(CacheKey);
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            IDictionaryEnumerator enumCache = objCache.GetEnumerator();
            while (enumCache.MoveNext())
            {
                objCache.Remove(enumCache.Key.ToString());
            }
        }
    }
}
