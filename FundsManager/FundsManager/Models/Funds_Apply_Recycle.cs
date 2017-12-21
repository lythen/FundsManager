using System;
using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 订单回收站
    /// </summary>
    public class Funds_Apply_Recycle
    {
        [Key]
        public int id { get; set; }
        [StringLength(9)]
        public string apply_number { get; set; }
        public int apply_user_id { get; set; }
        public DateTime apply_time { get; set; }
        public int apply_funds_id { get; set; }
        [StringLength(2000)]
        public string apply_for { get; set; }
        [DataType(DataType.Currency)]
        public decimal apply_amount { get; set; }
        public int apply_state { get; set; }
        public int apply_delete_user { get; set; }
        public DateTime apply_delete_time { get; set; }
    }
}