using FundsManager.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FundsManager.Common;
namespace FundsManager.ViewModels
{
    public class FundsInfo
    {
        public int id { get; set; }
        [StringLength(100),DisplayName("经费名称")]
        public string name { get; set; }
        [StringLength(4), DisplayName("使用年度")]
        public string year { get; set; }
        [DisplayName("结算时间")]
        public DateTime expireDate { get; set; }
        [StringLength(100), DisplayName("经费来源")]
        public string source { get; set; }
        [DataType(DataType.Currency), DisplayName("经费总额")]
        public decimal amount { get; set; }
        [DataType(DataType.Currency), DisplayName("经费余额")]
        public decimal balance { get; set; }
        public int manager { get; set; }
        [DisplayName("经费管理员")]
        public string managerName { get; set; }
        [StringLength(2000), DisplayName("备注")]
        public string info { get; set; }
        public Funds toDBModel()
        {
            Funds model = new Funds();
            model.f_amount = amount;
            model.f_balance = balance;
            model.f_expireDate = expireDate;
            model.f_id = id;
            model.f_info = PageValidate.InputText(info, 2000);
            model.f_in_year = year;
            model.f_manager = manager;
            model.f_name = PageValidate.InputText(name, 100);
            model.f_source = PageValidate.InputText(source, 100);
            return model;
        }
    }
}