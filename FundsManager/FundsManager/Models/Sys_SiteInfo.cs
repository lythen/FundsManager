using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    public class Sys_SiteInfo
    {
        [StringLength(100),Key]
        public string site_name { get; set; }
        [StringLength(100)]
        public string site_company { get; set; }
        [StringLength(2000)]
        public string site_introduce { get; set; }
        [StringLength(200)]
        public string site_company_address { get; set; }
        [StringLength(20)]
        public string site_company_phone { get; set; }
        [StringLength(100)]
        public string site_company_email { get; set; }
        [StringLength(50)]
        public string site_manager_name { get; set; }
        [StringLength(20)]
        public string site_manager_phone { get; set; }
        [StringLength(100)]
        public string site_manager_email { get; set; }
    }
}