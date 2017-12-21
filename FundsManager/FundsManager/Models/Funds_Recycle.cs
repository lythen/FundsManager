using System;
using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 经费回收站
    /// </summary>
    public class Funds_Recycle
    {
        [Key]
        public int id { get; set; }
        public int f_id { get; set; }
        [StringLength(100)]
        public string f_name { get; set; }
        [StringLength(4)]
        public string f_in_year { get; set; }
        public DateTime f_expireDate { get; set; }
        [StringLength(100)]
        public string f_source { get; set; }
        [DataType(DataType.Currency)]
        public decimal f_amount { get; set; }
        [DataType(DataType.Currency)]
        public decimal f_balance { get; set; }
        public int f_manager { get; set; }
        [StringLength(2000)]
        public string f_info { get; set; }
        public int f_delete_user { get; set; }
        public int f_delete_time { get; set; }
    }
}