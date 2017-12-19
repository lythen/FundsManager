using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FundsManager.ViewModels
{
    public class SystemSet
    {
    }
    public class SiteInfo
    {
        [StringLength(100), DisplayName("网站名称")]
        public string name { get; set; }
        [StringLength(100), DisplayName("所属公司/单位")]
        public string company { get; set; }
        [StringLength(2000), DisplayName("网站介绍")]
        public string introduce { get; set; }
        [StringLength(200), DisplayName("公司/单位地址")]
        public string companyAddress { get; set; }
        [StringLength(20), DisplayName("公司/单位电话")]
        public string companyPhone { get; set; }
        [StringLength(100), DisplayName("公司/单位邮箱")]
        public string companyEmail { get; set; }
        [StringLength(50), DisplayName("管理员姓名")]
        public string managerName { get; set; }
        [StringLength(20), DisplayName("管理员电话")]
        public string manaterPhone { get; set; }
        [StringLength(100), DisplayName("管理员邮箱")]
        public string managerEmail { get; set; }
    }
}