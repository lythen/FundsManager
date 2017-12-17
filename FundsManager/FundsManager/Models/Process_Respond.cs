using System;
using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 流程批复表
    /// </summary>
    public class Process_Respond
    {
        private DateTime _pr_time = DateTime.Now;
        [Key]
        public int pr_id { get; set; }
        [StringLength(9)]
        public string pr_apply_number { get; set; }
        public int pr_user_id { get; set; }
        public int pr_number { get; set; }
        public DateTime pr_time { get { return _pr_time; } set { _pr_time = value; } }
        [StringLength(2000)]
        public string pr_content { get; set; }
        public int pr_state { get; set; }
    }
}