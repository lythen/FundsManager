using System;
using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 需要报销的经费细节
    /// </summary>
    public class Reimbursement_Detail
    {
        [Key]
        public int detail_id { get; set; }
        public int detail_content_id { get; set; }
        [StringLength(200)]
        public string detail_info { get; set; }
        [DataType(DataType.Currency)]
        public decimal detail_amount { get; set; }
        public DateTime detail_date { get; set; }
    }
}