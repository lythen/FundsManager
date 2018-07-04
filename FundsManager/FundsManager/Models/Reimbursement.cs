using System;
using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 经费申请总表
    /// </summary>
    public class Reimbursement
    {
        private DateTime _add_time = DateTime.Now;
        private int _apply_state = 0;
        /// <summary>
        /// 报销单号
        /// </summary>
        [Key,StringLength(9)]
        public string reimbursement_code { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        public int r_add_user_id { get; set; }
        /// <summary>
        /// 填表时间
        /// </summary>
        public DateTime r_add_time { get { return _add_time; }set { _add_time = value; } }
        /// <summary>
        /// 报销金额合计
        /// </summary>
        [DataType(DataType.Currency)]
        public decimal r_bill_amount { get; set; }
        /// <summary>
        /// 批复状态
        /// </summary>
        public int r_bill_state { get { return _apply_state; } set { _apply_state = value; } }
        /// <summary>
        /// 报销事由
        /// </summary>
        public string reimbursement_info { get; set; }
        /// <summary>
        /// 开支项目
        /// </summary>
        public int r_funds_id { get; set; }
        /// <summary>
        /// 实际领取金额
        /// </summary>
        public decimal r_fact_amount { get; set; }
    }
}