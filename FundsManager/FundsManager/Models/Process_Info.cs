using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FundsManager.Models
{
    /// <summary>
    /// 批复流程
    /// </summary>
    public class Process_Info
    {
        private int _process_funds = 0;
        [Key]
        public int process_id { get; set; }
        [StringLength(50)]
        public string process_name { get; set; }
        [DisplayName("创建人")]
        public int process_user_id { get; set; }
        public DateTime process_create_time { get; set; }
        [DisplayName("默认使用该流程的经费")]
        public int process_funds { get { return _process_funds; } set { _process_funds = value; } }
    }
}