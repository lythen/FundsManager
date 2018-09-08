using System.ComponentModel.DataAnnotations;
using System;
namespace Lythen.Models
{
    /// <summary>
    /// 经费表
    /// </summary>
    public class Funds
    {
        private DateTime _f_add_Time = DateTime.Now;
        [Key]
        public int f_id { get; set; }
        [StringLength(20)]
        public string f_code { get; set; }
        [StringLength(100)]
        public string f_name { get; set; }
        [StringLength(100)]
        public string f_source { get; set; }
        [DataType(DataType.Currency)]
        public decimal f_amount { get; set; }
        [DataType(DataType.Currency)]
        public decimal f_balance { get; set; }
        public int f_manager { get; set; }
        [StringLength(2000)]
        public string f_info { get; set; }
        public int f_state { get; set; }
        public DateTime f_add_Time { get { return _f_add_Time; } set { _f_add_Time = value; } }
        public int? f_process { get; set; }
    }
}