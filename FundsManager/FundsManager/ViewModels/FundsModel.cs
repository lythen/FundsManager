using FundsManager.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FundsManager.Common;
using System.Collections.Generic;

namespace FundsManager.ViewModels
{
    public class FundsModel : FundsBaseModel
    {
        private int _state = 1;
        [StringLength(4), DisplayName("到期时间")]
        public string year { get; set; }
        [DisplayName("经费管理员")]
        public int manager { get; set; }
        [StringLength(2000), DisplayName("备注")]
        public string info { get; set; }
        [DisplayName("状态")]
        public int state { get { return _state; } set { _state = value; } }
        public void toDBModel(Funds model)
        {
            model.f_amount = amount;
            model.f_balance = balance == null ? amount : (decimal)balance;
            model.f_expireDate = expireDate;
            if (model.f_id == 0)
                model.f_id = id;
            model.f_info = PageValidate.InputText(info, 2000);
            model.f_in_year = expireDate.Year.ToString();
            model.f_manager = manager;
            model.f_name = PageValidate.InputText(name, 100);
            model.f_source = PageValidate.InputText(source, 100);
            model.f_state = state;
        }
    }
    public class FundsBaseModel
    {
        private DateTime _expireDate = DateTime.Parse(string.Format("{0}-12-31 23:59:59.999", DateTime.Now.Year));
        [DisplayName("经费ID")]
        public int id { get; set; }
        [StringLength(100), DisplayName("经费名称")]
        public string name { get; set; }
        [DisplayName("经费管理员")]
        public string managerName { get; set; }
        [DisplayName("结算时间")]
        public DateTime expireDate { get { return _expireDate; } set { _expireDate = value == null ? _expireDate : value; } }
        [StringLength(100), DisplayName("经费来源")]
        public string source { get; set; }
        [DataType(DataType.Currency), DisplayName("经费总额")]
        public decimal amount { get; set; }
        [DataType(DataType.Currency), DisplayName("经费余额")]
        public decimal? balance { get; set; }
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
    public class ProcessModel
    {
        private int _funds = 0;
        private DateTime _time = DateTime.Now;
        [DisplayName("流程ID")]
        public int id { get; set; }
        [StringLength(50),DisplayName("流程名称")]
        public string name { get; set; }
        [DisplayName("创建人")]
        public int user { get; set; }
        [DisplayName("创建时间")]
        public DateTime time { get { return _time; } set { _time = value; } }
        [DisplayName("默认使用")]
        public int funds { get { return _funds; } set { _funds = value; } }
        [DisplayName("详细流程")]
        public List<ProcessDetail> processList { get; set; }
    }
    public class ProcessDetail
    {
        public int id { get; set; }
        [DisplayName("批复领导")]
        public int user { get; set; }
        [DisplayName("批复序号")]
        public int sort { get; set; }
    }
}