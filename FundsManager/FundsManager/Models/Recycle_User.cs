using System;
using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    public class Recycle_User
    {
        [Key]
        public int delete_id { get; set; }
        public int user_id { get; set; }
        [StringLength(20), Required]
        public string user_name { get; set; }
        [StringLength(100)]
        public string real_name { get; set; }
        [StringLength(50)]
        public string user_certificate_type { get; set; }
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
        public int user_state { get; set; }
        public int user_login_times { get ;  set ; }
        [StringLength(2)]
        public string user_gender { get; set; }
        public int user_post_id { get; set; }
        [StringLength(20)]
        public string user_office_phone { get; set; }
        [StringLength(50)]
        public string user_picture { get; set; }
        public int user_dept_id { get; set; }
        public DateTime? user_add_time { get; set; }
        public int? user_add_user { get; set; }
        public DateTime? user_edit_time { get; set; }
        public int? user_edit_user { get; set; }
        public void FromUserInfo(User_Info info)
        {
            user_id = info.user_id;
            user_name = info.user_name;
            real_name = info.real_name;
            user_certificate_no = info.user_certificate_no;
            user_certificate_type = info.user_certificate_type;
            user_mobile = info.user_mobile;
            user_email = info.user_email;
            user_password = info.user_password;
            user_salt = info.user_salt;
            user_state = info.user_state;
            user_login_times = info.user_login_times;
        }
        public void FromUserExtend(User_Extend info)
        {
            if (info == null) return;
            user_gender = info.user_gender;
            user_post_id = info.user_post_id;
            user_office_phone = info.user_office_phone;
            user_picture = info.user_picture;
            user_dept_id = info.user_dept_id;
            user_add_user = info.user_add_user;
            user_add_time = info.user_add_time;
            user_edit_time = info.user_edit_time;
            user_edit_user = info.user_edit_user;
        }
    }
}