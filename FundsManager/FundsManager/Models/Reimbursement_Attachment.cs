using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 上传的附件
    /// </summary>
    public class Reimbursement_Attachment
    {
        private int _detail_id = 0;
        private string _reimbursement_code = "";
        [Key]
        public int attachment_id { get; set; }
        [StringLength(9)]
        public string atta_reimbursement_code { get { return _reimbursement_code; } set { _reimbursement_code = value; } }
        public int atta_detail_id { get { return _detail_id; } set { _detail_id = value; } }
        [StringLength(200)]
        public string attachment_path { get; set; }
    }
}