using System;
using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 日志表
    /// </summary>
    public class Sys_Log
    {
        private DateTime _log_time = DateTime.Now;
        [Key]
        public int log_id { get; set; }
        public int log_user_id { get; set; }
        [StringLength(100)]
        public string log_target { get; set; }
        [StringLength(2000)]
        public string log_content { get; set; }
        public int log_type { get; set; }
        public DateTime log_time { get {return _log_time; } set { _log_time = value; } }
        [StringLength(150)]
        public string log_ip { get; set; }
        [StringLength(500)]
        public string log_device { get; set; }

    }
}