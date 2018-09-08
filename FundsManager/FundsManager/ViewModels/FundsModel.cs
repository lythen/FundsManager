using Lythen.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Lythen.Common;
using System.Collections.Generic;

namespace Lythen.ViewModels
{
    public class FundsModel : FundsBaseModel
    {
        private int _state = 1;
        [StringLength(2000), DisplayName("备注")]
        public string info { get; set; }
        [DisplayName("状态")]
        public int state { get { return _state; } set { _state = value; } }
        public void toDBModel(Funds model)
        {
            model.f_amount = amount;
            model.f_balance = balance == null ? amount : (decimal)balance;
            if (model.f_id == 0)
                model.f_id = id;
            model.f_info = PageValidate.InputText(info, 2000);
            model.f_name = PageValidate.InputText(name, 100);
            model.f_source = PageValidate.InputText(source, 100);
            model.f_state = state;
            model.f_code = code;
        }
    }
    public class FundsBaseModel
    {
        [DisplayName("经费ID")]
        public int id { get; set; }
        [StringLength(20), DisplayName("经费代码"),Required]
        public string code { get; set; }
        [StringLength(100), DisplayName("经费名称")]
        public string name { get; set; }
        [DisplayName("经费管理员")]
        public string managerName { get; set; }
        [StringLength(100), DisplayName("经费来源")]
        public string source { get; set; }
        [DataType(DataType.Currency), DisplayName("经费总额")]
        public decimal amount { get; set; }
        [DataType(DataType.Currency), DisplayName("经费余额")]
        public decimal? balance { get; set; }
        public int? manager { get; set; }
    }
    public class mFundsListModel : FundsBaseModel
    {
        [DisplayName("状态")]
        public string strState { get; set; }
        [DisplayName("已申请数")]
        public int userCount { get; set; }
        [DisplayName("申请总额")]
        public decimal applyamount { get; set; }
    }
    public class uFundsListModel : FundsBaseModel
    {

    }
    public class FundsListView
    {
        //管理的经费
        public List<mFundsListModel> managerFunds { get; set; }
        //使用的经费
        public List<uFundsListModel> useFunds { get; set; }
    }
    public class FundsStatDetail
    {
        [DisplayName("经费名称")]
        public string fname { get; set; }
        [DisplayName("申请人")]
        public string uname { get; set; }
        [DisplayName("申请金额")]
        public decimal applyAmount { get; set; }
        [DisplayName("申请时间")]
        public DateTime applyTime { get; set; }
    }
    public class FundsStat
    {
        public int id { get; set; }
        [DisplayName("经费名称")]
        public string name { get; set; }
        [DisplayName("经费总额（元）")]
        public decimal amount { get; set; }
        [DisplayName("经费已用（元）")]
        public decimal hasUsed { get; set; }
        [DisplayName("经费余额（元）")]
        public decimal balance { get; set; }
        [DisplayName("申请数量（笔）")]
        public int applyNum { get; set; }
    }
}