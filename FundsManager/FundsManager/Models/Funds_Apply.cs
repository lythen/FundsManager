using System;
using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 经费申请总表
    /// </summary>
    public class Funds_Apply
    {
        private DateTime _apply_time = DateTime.Now;
        private int _apply_state = 0;
        [Key,StringLength(9)]
        public string apply_number { get; set; }
        public int apply_user_id { get; set; }
        public DateTime apply_time { get { return _apply_time; }set { _apply_time = value; } }
        public int apply_funds_id { get; set; }
        [StringLength(2000)]
        public string apply_for { get; set; }
        [DataType(DataType.Currency)]
        public decimal apply_amount { get; set; }
        public int apply_state { get { return _apply_state; } set { _apply_state = value; } }
    }
}