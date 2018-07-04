using FundsManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FundsManager.ViewModels
{
    public class ReimbursementModel
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
    public class ApplyListModel: ReimbursementModel
    {
        [DisplayName("批复人")]
        public int next { get; set; }
        public List<ViewContentModel> child { get; set; }
    }
    public class ChildState
    {
        public string childState { get; set; }
    }
    public class ViewContentModel
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
    /// <summary>
    /// 申请单详细信息
    /// </summary>
    public class ApplyDetail: ReimbursementModel
    {
        public List<ApplyChildModel> applyList {get;set;}
    }
}