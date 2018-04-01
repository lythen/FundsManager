﻿using System;
using System.ComponentModel;

namespace FundsManager.ViewModels
{
    public class ListResponseModel
    {
        private DateTime? _pr_time = DateTime.Now;
        [DisplayName("申请序号")]
        public int id { get; set; }
        [DisplayName("子单号")]
        public string capply_number { get; set; }
        [DisplayName("批复人")]
        public string user { get; set; }
        public int number { get; set; }
        public DateTime? time { get { return _pr_time; } set { _pr_time = value; } }
        [DisplayName("批复时间")]
        public string strTime { get { return time==null?"":((DateTime)time).ToString("yyyy年MM月dd日 HH时mm分"); } }
        [DisplayName("批复说明")]
        public string content { get; set; }
        [DisplayName("批复状态")]
        public string strState { get; set; }
        public int state { get; set; }
    }
    public class ResponseDetail: ListResponseModel
    {
        [DisplayName("经费编号")]
        public string fundsCode { get; set; }
        [DisplayName("申请单号")]
        public string apply_number { get; set; }
        [DisplayName("申请名")]
        public string name { get; set; }
        [DisplayName("申请时间")]
        public string strAddDate { get { return addDate == null ? "" : ((DateTime)addDate).ToString("yyyy-MM-dd HH:mm"); }  }
        public DateTime? addDate { get; set; }
        [DisplayName("申请人")]
        public string applyUser { get; set; }
        [DisplayName("申请金额")]
        public decimal applyAmount { get; set; }
    }
    public class Respond
    {
        public string agree { get; set; }
        public string reason { get; set; }
        public int? id { get; set; }
        public int? next { get; set; }
    }
}