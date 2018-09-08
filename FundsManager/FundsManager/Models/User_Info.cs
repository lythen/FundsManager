using System.ComponentModel.DataAnnotations;
using Lythen.Common.DEncrypt;
using System;

namespace Lythen.Models
{
    /// <summary>
    /// 用户表
    /// </summary>
    [Serializable]
    public sealed class User_Info
    {
        private int _user_state = 0;
        private int _user_login_times = 0;
        [Key]
        public int user_id { get; set; }
        [StringLength(20),Required]
        public string user_name { get; set; }
        [StringLength(100)]
        public string real_name { get; set; }
        [StringLength(50)]
        public string user_certificate_type{get;set;}
        [StringLength(100)]
        public string user_certificate_no { get; set; }
        [StringLength(100)]
        public string user_mobile { get; set; }
        [StringLength(200)]
        public string user_email { get; set; }
        [StringLength(200)]
        public string user_password { get; set; }
        [StringLength(10)]
        public string user_salt { get; set; }
        public int user_state { get { return _user_state; } set { _user_state = value; } }
        public int user_login_times { get { return _user_login_times; } set { _user_login_times = value; } }
        public void ToEncrypt()
        {
            if (!string.IsNullOrEmpty(user_certificate_no))
                user_certificate_no = AESEncrypt.Encrypt(user_certificate_no);
            if (!string.IsNullOrEmpty(user_password))
                user_password = AESEncrypt.Encrypt(user_password);
            if (!string.IsNullOrEmpty(real_name))
                real_name = AESEncrypt.Encrypt(real_name);
            if (!string.IsNullOrEmpty(user_mobile))
                user_mobile = AESEncrypt.Encrypt(user_mobile);
            if (!string.IsNullOrEmpty(user_email))
                user_email = AESEncrypt.Encrypt(user_email);
        }
        public void ToDecrypt()
        {
            if (!string.IsNullOrEmpty(user_certificate_no))
                user_certificate_no = AESEncrypt.Decrypt(user_certificate_no);
            if (!string.IsNullOrEmpty(user_password))
                user_password = AESEncrypt.Decrypt(user_password);
            if (!string.IsNullOrEmpty(real_name))
                real_name = AESEncrypt.Decrypt(real_name);
            if (!string.IsNullOrEmpty(user_mobile))
                user_mobile = AESEncrypt.Decrypt(user_mobile);
            if (!string.IsNullOrEmpty(user_email))
                user_email = AESEncrypt.Decrypt(user_email);

        }
        public void DeletePassword()
        {
            user_password = "";
        }
    }
}