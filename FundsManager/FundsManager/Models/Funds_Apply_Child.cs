using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 经费子申请表
    /// </summary>
    public class Funds_Apply_Child
    {
        [StringLength(9)]
        public string c_apply_number { get; set; }
        [StringLength(13),Key]
        public string c_child_number { get; set; }
        [Required,DataType(DataType.Currency)]
        public decimal c_amount { get; set; }
    }
}