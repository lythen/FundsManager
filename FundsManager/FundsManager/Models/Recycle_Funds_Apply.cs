using System;
using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 订单回收站
    /// </summary>
    public class Recycle_Funds_Apply
    {
        private int _apply_state = 0;
        [Key, StringLength(9)]
        public string apply_number { get; set; }
        public int apply_user_id { get; set; }
        public DateTime apply_time { get; set; }
        [DataType(DataType.Currency)]
        public decimal apply_amount { get; set; }
        public int apply_state { get { return _apply_state; } set { _apply_state = value; } }
    }
}