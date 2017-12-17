using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class User_Info
    {
        private int _user_state = 0;
        private int _user_login_times = 0;
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
        public int user_state { get { return _user_state; } set { _user_state = value; } }
        public int user_login_times { get { return _user_login_times; } set { _user_login_times = value; } }
    }
}