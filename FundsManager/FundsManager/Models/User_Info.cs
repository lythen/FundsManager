using System.ComponentModel.DataAnnotations;
using FundsManager.Common.DEncrypt;
namespace FundsManager.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class User_Info
    {
        private int _user_state = 0;
        private int _user_login_times = 0;
        private string _user_name;
        private string _user_mobile;
        private string _user_email;
        private string _user_certificate_no;
        private string _user_password;
        [Key]
        public int user_id { get; set; }
        [StringLength(20),Required]
        public string user_name { get; set; }
        [StringLength(100)]
        public string real_name { get {return _user_name; } set { _user_name = string.IsNullOrEmpty(value) ? "" : AESEncrypt.Encrypt(value); } }
        [StringLength(50)]
        public string user_certificate_type{get;set;}
        [StringLength(100)]
        public string user_certificate_no { get { return _user_certificate_no; } set { _user_certificate_no = string.IsNullOrEmpty(value) ? "" : AESEncrypt.Encrypt(value); } }
        [StringLength(100)]
        public string user_mobile { get { return _user_mobile; } set { _user_mobile = string.IsNullOrEmpty(value) ? "" : AESEncrypt.Encrypt(value); } }
        [StringLength(200)]
        public string user_email { get { return _user_email; } set { _user_email = string.IsNullOrEmpty(value) ? "" : AESEncrypt.Encrypt(value); } }
        [StringLength(200)]
        public string user_password { get { return _user_password; } set { _user_password = string.IsNullOrEmpty(value) ? "" : AESEncrypt.Encrypt(value); } }
        [StringLength(10)]
        public string user_salt { get; set; }
        public int user_state { get { return _user_state; } set { _user_state = value; } }
        public int user_login_times { get { return _user_login_times; } set { _user_login_times = value; } }
    }
}