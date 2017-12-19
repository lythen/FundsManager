using FundsManager.DAL;
using System;
using System.Linq;
using FundsManager.Common;
using FundsManager.ViewModels;
namespace FundsManager.Controllers
{
    public static class SiteInfo
    {
        private static FundsContext db = new FundsContext();
        public static string getSiteName()
        {
            object siteName = DataCache.GetCache("site-name");
            if (siteName == null)
            {
                string name = (from x in db.Sys_SiteInfo
                               select x.site_name
                               ).FirstOrDefault();
                DataCache.SetCache("site-name", name, DateTime.Now.AddYears(3), System.Web.Caching.Cache.NoSlidingExpiration);
                return name;
            }
            else return (string)siteName;
        }
        public static ViewModels.SiteInfo getSiteInfo()
        {
            object siteName = DataCache.GetCache("site-info");
            if (siteName == null)
            {
                var info = (from x in db.Sys_SiteInfo
                            select new ViewModels.SiteInfo
                            {
                                company = x.site_company,
                                companyAddress = x.site_company_address,
                                companyEmail = x.site_company_email,
                                companyPhone = x.site_company_phone,
                                introduce = x.site_introduce,
                                managerEmail = x.site_manager_email,
                                managerName = x.site_manager_name,
                                manaterPhone = x.site_manager_phone,
                                name = x.site_name
                            }
                               ).FirstOrDefault();
                DataCache.SetCache("site-info", info, DateTime.Now.AddYears(3), System.Web.Caching.Cache.NoSlidingExpiration);
                return info;
            }
            else return (ViewModels.SiteInfo)siteName;
        }
    }
}