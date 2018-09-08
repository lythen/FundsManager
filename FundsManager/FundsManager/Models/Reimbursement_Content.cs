using System.ComponentModel.DataAnnotations;

namespace Lythen.Models
{
    /// <summary>
    /// 报销内容
    /// </summary>
    public class Reimbursement_Content
    {
        private int _c_funds_id = 0;
        /// <summary>
        /// 报销单号
        /// </summary>
        [StringLength(9)]
        public string c_reimbursement_code { get; set; }
        /// <summary>
        /// 报销内容流水ID
        /// </summary>
        [Key]
        public int content_id { get; set; }
        /// <summary>
        /// 报销使用的经费，默认不填，保留字段
        /// </summary>
        public int c_funds_id { get { return _c_funds_id; } set { _c_funds_id = value; } }
        /// <summary>
        /// 报销金额
        /// </summary>
        [Required,DataType(DataType.Currency)]
        public decimal c_amount { get; set; }
        /// <summary>
        /// 对应的报销内容
        /// </summary>
        public int c_dic_id{ get; set; }
    }
}