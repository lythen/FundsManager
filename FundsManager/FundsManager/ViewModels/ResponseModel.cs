using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FundsManager.ViewModels
{
    public class ResponseModel: ListResponseModel
    {
        
        [DisplayName("申请说明")]
        public string applyInfo { get; set; }
        public int state { get; set; }
    }
    public class ListResponseModel
    {
        [DisplayName("申请单号")]
        public string CNumber { get; set; }
        [DisplayName("申请人")]
        public string applyUser { get; set; }
        [DisplayName("申请时间")]
        public DateTime applyTime { get; set; }
        [DisplayName("申请经费")]
        public string fundsName { get; set; }
        [DisplayName("申请金额")]
        public decimal applyAmount { get; set; }
        [DisplayName("当前状态")]
        public string strState { get; set; }
    }
}