using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FundsManager.Models;
using FundsManager.Common;
using System.Collections.Generic;
namespace FundsManager.ViewModels
{
    public class SystemSetModel
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
        public string managerPhone { get; set; }
        [StringLength(100), DisplayName("管理员邮箱")]
        public string managerEmail { get; set; }
        public Sys_SiteInfo toDBModel(Sys_SiteInfo model)
        {
            model.site_name = PageValidate.InputText(name, 100);
            model.site_company = PageValidate.InputText(company, 100);
            model.site_company_address = PageValidate.InputText(companyAddress, 1200);
            model.site_company_email = PageValidate.InputText(companyEmail, 100);
            model.site_company_phone = PageValidate.InputText(companyPhone, 20);
            model.site_introduce = PageValidate.InputText(introduce, 2000);
            model.site_manager_email = PageValidate.InputText(managerEmail, 100);
            model.site_manager_name = PageValidate.InputText(managerName, 50);
            model.site_manager_phone = PageValidate.InputText(managerPhone, 20);
            return model;
        }
    }
    public class ModuleInfo
    {
        public int id { get; set; }
        [StringLength(20)]
        public string name { get; set; }
        [StringLength(1000)]
        public string info { get; set; }
        public int hasChange = 0;
        public RoleInfo[] roles { get; set; }
    }
    public class EditModules
    {
        public List<ModuleInfo> modules { get; set; }
    }
    public class RoleInfo
    {
        public int id { get; set; }
        [StringLength(20)]
        public string name { get; set; }
        public bool hasrole { get; set; }
    }
    public class RoleAuthority
    {
        public int roleId { get; set; }
        public int authId { get; set; }
    }
    public class AuthInfo
    {
        public int authId { get; set; }
        public string authName { get; set; }
        public string authInfo { get; set; }
        public bool isController { get; set; }
        public int mapController { get; set; }
    }
    public class DepartMentModel
    {
        private int parent_id = 0;
        [DisplayName("部门ID")]
        public int? deptId { get; set; }
        [DisplayName("部门名称")]
        public string deptName { get; set; }
        [DisplayName("上级部门ID")]
        public int parentId { get { return parent_id; } set { parent_id = value; } }
        [DisplayName("上级部门名称")]
        public string parentName { get; set; }
    }
}