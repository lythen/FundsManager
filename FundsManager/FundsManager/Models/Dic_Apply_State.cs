using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 申请状态表
    /// </summary>
    public class Dic_Apply_State
    {
        [Key]
        public int das_state_id { get; set; }
        [StringLength(20),Required]
        public string das_state_name { get; set; }
    }
}