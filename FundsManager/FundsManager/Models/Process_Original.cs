using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 流程表
    /// </summary>
    public class Process_Original
    {
        [Key]
        public int po_id { get; set; }
        public int po_f_id { get; set; }
        public int po_user_id { get; set; }
        public int po_number { get; set; }
    }
}