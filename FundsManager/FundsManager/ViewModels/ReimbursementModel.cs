using FundsManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FundsManager.ViewModels
{
    /// <summary>
    /// 报帐单
    /// </summary>
    public class ReimbursementModel
    {
        private DateTime _apply_time = DateTime.Now;
        private int _apply_state = 0;
        [StringLength(9),DisplayName("报销单编号")]
        public string nureimbursementCodember { get; set; }
        public int userId { get; set; }
        [DisplayName("填表时间")]
        public DateTime time { get { return _apply_time; } set { _apply_time = value; } }
        [DataType(DataType.Currency),DisplayName("合计金额")]
        public decimal amount { get; set; }
        [DataType(DataType.Currency), DisplayName("实领金额")]
        public decimal factAmount { get; set; }
        public int state { get { return _apply_state; } set { _apply_state = value; } }
        public string strState { get; set; }
        [DisplayName("开支项目ID")]
        public int Fid { get; set; }
        [DisplayName("开支项目代码")]
        public string fundsCode { get; set; }
        
    }
    /// <summary>
    /// 报销单填写内容
    /// </summary>
    public class ApplyListModel: ReimbursementModel
    {
        [DisplayName("批复人")]
        public int next { get; set; }
        public List<string> attachments { get; set; }
        public List<ViewContentModel> contents { get; set; }
    }
    public class ChildState
    {
        public string childState { get; set; }
    }
    /// <summary>
    /// 报销内容
    /// </summary>
    public class ViewContentModel
    {
        private int _apply_state = 0;
        [StringLength(9), DisplayName("报销单编号")]
        public string reimbursementCode { get; set; }
        [StringLength(13), DisplayName("报销内容ID")]
        public int? contentId { get; set; }
        [Required, DataType(DataType.Currency), DisplayName("金额")]
        public decimal amount { get; set; }
        List<ViewDetailContent> details { get; set; }
    }
    /// <summary>
    /// 明细
    /// </summary>
    public class ViewDetailContent
    {
        public int? contentId { get; set; }
        public int? detailId { get; set; }
        [StringLength(200),Required]
        public string detailInfo { get; set; }
        [DataType(DataType.Currency), DisplayName("明细金额")]
        public decimal amount { get; set; }
        [DisplayName("日期")]
        public DateTime detail_date { get; set; }
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
        public List<ViewContentModel> contentList {get;set;}
    }
}