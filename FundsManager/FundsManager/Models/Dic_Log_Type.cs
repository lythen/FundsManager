using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 日志操作类别表
    /// </summary>
    public class Dic_Log_Type
    {
        [Key]
        public int dlt_log_id { get; set; }
        [StringLength(20), Required]
        public string dlt_log_name { get; set; }
    }
}