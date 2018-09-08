using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lythen.ViewModels
{
    /// <summary>
    /// 报帐单
    /// </summary>
    public class ReimbursementModel
    {
        private DateTime _apply_time = DateTime.Now;
        private int _apply_state = 0;
        [StringLength(9),DisplayName("报销单编号")]
        public string reimbursementCode { get; set; }
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
        [DisplayName("报销事由")]
        public string info { get; set; }
        
    }
    /// <summary>
    /// 报销单填写内容
    /// </summary>
    public class ApplyListModel: ReimbursementModel
    {
        [DisplayName("批复人")]
        public int next { get; set; }
        public string fundsName { get; set; }
        public int? attachmentsCount { get; set; }
        public List<ViewAttachment> attachments { get; set; }
        public List<ViewContentModel> contents { get; set; }
        public List<Respond> responds { get; set; }
        public string userName { get; set; }
        public int? manager { get; set; }
        public int? detailsCount { get; set; }
        public string myRespond { get; set; }
    }
    /// <summary>
    /// 附件内容
    /// </summary>
    public class ViewAttachment
    {
        public int? id { get; set; }
        public string reimbursementCode { get; set; }
        public string fileName { get; set; }
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
        [StringLength(9), DisplayName("报销单编号")]
        public string reimbursementCode { get; set; }
        [DisplayName("报销内容ID")]
        public int? contentId { get; set; }//不是字典的报销内容的ID（select）
        [Required, DataType(DataType.Currency), DisplayName("金额")]
        public decimal amount { get; set; }
        public List<ViewDetailContent> details { get; set; }
        public int selectId { get; set; }//传入的select的value
        public string contentTitle { get; set; }
        public List<ViewAttachment> attachments { get; set; }
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
        [DisplayName("日期"),DisplayFormat(DataFormatString = "{0:d}",NullDisplayText ="")]
        public DateTime? detailDate { get; set; }
        public string strDate { get; set; }
        public string contentTitle { get; set; }
    }
    public class ApplyFundsManager
    {
        [StringLength(9), DisplayName("报销单编号")]
        public string reimbursementCode { get; set; }
        [DisplayName("管理员")]
        public string strManager { get; set; }
        [DisplayName("报销单状态")]
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
    public class BillsSearchModel : BasePagerModel
    {
        private int? _userId = 0;
        private int? _fid = 0;
        [DisplayName("状态")]
        public int? state { get ; set;  }
        [DisplayName("开始时间")]
        public string strBeginDate { get; set; }
        [DisplayName("结束时间")]
        public string strEndDate { get; set; }
        public int? userId { get { return _userId; } set { _userId = value; } }
        public int? fid { get { return _fid; }set { _fid = value; } }
        [DisplayFormat(DataFormatString =("{0:d}"),NullDisplayText ="")]
        public DateTime? beginDate { get; set; }
        [DisplayFormat(DataFormatString = ("{0:d}"), NullDisplayText = "")]
        public DateTime? endDate { get; set; }
        public string reimbursementCode { get; set; }
    }
    public class StatisticsSearch : BasePagerModel
    {
        public int? fund { get; set; }
        [DisplayFormat(DataFormatString = ("{0:d}"), NullDisplayText = "")]
        public DateTime? beginDate { get; set; }
        [DisplayFormat(DataFormatString = ("{0:d}"), NullDisplayText = "")]
        public DateTime? endDate { get; set; }
        public int? userId { get; set; }
        public int? manager { get; set; }
    }
}