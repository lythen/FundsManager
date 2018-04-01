using FundsManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FundsManager.ViewModels
{
    public class ApplyModel
    {
        private DateTime _apply_time = DateTime.Now;
        private int _apply_state = 0;
        [StringLength(9),DisplayName("申请序号")]
        public string number { get; set; }
        public int userId { get; set; }
        [DisplayName("申请时间")]
        public DateTime time { get { return _apply_time; } set { _apply_time = value; } }
        [DataType(DataType.Currency),DisplayName("总额")]
        public decimal amount { get; set; }
        public int state { get { return _apply_state; } set { _apply_state = value; } }
        public string strState { get; set; }
    }
    public class ApplyListModel: ApplyModel
    {
        [DisplayName("状态")]
        public List<ApplyChildModel> child { get; set; }
    }
    public class ChildState
    {
        public string childState { get; set; }
    }
    public class ApplyChildModel
    {
        private int _apply_state = 0;
        [StringLength(9), DisplayName("申请编号")]
        public string Fnumber { get; set; }
        [StringLength(13), DisplayName("子编号")]
        public string Cnumber { get; set; }
        [DisplayName("经费ID")]
        public int Fid { get; set; }
        [DisplayName("经费代码")]
        public string fundsCode { get; set; }
        [Required, DataType(DataType.Currency), DisplayName("金额")]
        public decimal amount { get; set; }
        [DisplayName("状态")]
        public int state { get { return _apply_state; } set { _apply_state = value; } }
        [DisplayName("状态")]
        public string strState { get; set; }
        [StringLength(2000),DisplayName("备注")]
        public string applyFor { get; set; }
        [DisplayName("实际领取")]
        public decimal? factGet { get; set; }
        [StringLength(2000), DisplayName("领取说明")]
        public string getInfo { get; set; }
    }
    public class ApplyFundsManager
    {
        [StringLength(13), DisplayName("子编号")]
        public string Cnumber { get; set; }
        [DisplayName("管理员")]
        public string strManager { get; set; }
        [DisplayName("申请单状态")]
        public string strState { get; set; }
        public List<ListResponseModel> processList { get; set; }
    }
    public class ApplyEditModel: ApplyModel
    {
        public int next { get; set; }
        public List<ApplyChildModel> capply { get; set; }
    }
    /// <summary>
    /// 申请单详细信息
    /// </summary>
    public class ApplyDetail: ApplyModel
    {
        public List<ApplyChildModel> applyList {get;set;}
    }
}