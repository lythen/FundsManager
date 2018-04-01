using System;
using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 流程批复表
    /// </summary>
    public class Process_Respond
    {
        private int _pr_state = 0;
        [Key]
        public int pr_id { get; set; }
        [StringLength(20)]
        public string pr_apply_number { get; set; }
        public int pr_user_id { get; set; }
        public int pr_number { get; set; }
        public DateTime? pr_time { get; set; }
        [StringLength(2000)]
        public string pr_content { get; set; }
        public int pr_state { get { return _pr_state; } set { _pr_state = value; } }
        public int? next { get; set; }
    }
}