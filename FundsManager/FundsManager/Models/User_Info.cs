using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class User_Info
    {
        [Key]
        public int user_id { get; set; }
        [StringLength(50)]
        public string user_name { get; set; }
        [StringLength(50)]
        public string user_certificate_type{get;set;}
        [StringLength(20)]
        public string user_certificate_no { get; set; }
        [StringLength(20)]
        public string user_mobile { get; set; }
        [StringLength(100)]
        public string user_email { get; set; }
        [StringLength(200)]
        public string user_password { get; set; }
        [StringLength(10)]
        public string user_salt { get; set; }
    }
}