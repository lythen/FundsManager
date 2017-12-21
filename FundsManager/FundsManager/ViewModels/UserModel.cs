using FundsManager.Common.DEncrypt;
using FundsManager.Models;
using System.ComponentModel.DataAnnotations;
using FundsManager.Common;
using System.ComponentModel;

namespace FundsManager.ViewModels
{
    public class UserModel
    {
        public int? id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [DisplayName("用户名"), Required]
        public string name { get; set; }
        /// <summary>
        /// 上一次登陆时间
        /// </summary>
        [DisplayName("登陆时间")]
        public string lastTime { get; set; }
        /// <summary>
        /// 上一次登陆IP
        /// </summary>
        [DisplayName("登陆IP")]
        public string lastIp { get; set; }
        /// <summary>
        /// 上一次登陆设备
        /// </summary>
        [DisplayName("登陆设备")]
        public string lastDev { get; set; }
        /// <summary>
        /// 登陆次数
        /// </summary>
        [DisplayName("登陆次数")]
        public int times { get; set; }
        /// <summary>
        /// 登陆角色
        /// </summary>
        [DisplayName("角色名")]
        public string roleName { get; set; }
    }
    public class UserEditModel: UserModel
    {
        [StringLength(20),DisplayName("真实姓名"),Required]
        public string realName { get; set; }
        [StringLength(50), DisplayName("证件类别")]
        public string certificateType { get; set; }
        [StringLength(20), DisplayName("证件号码")]
        public string certificateNo { get; set; }
        [StringLength(20), DisplayName("手机号码"), Required]
        public string mobile { get; set; }
        [StringLength(200), DisplayName("电子邮箱"), Required]
        public string email { get; set; }
        [StringLength(16), DisplayName("登陆密码"), Required]
        public string password { get; set; }
        [StringLength(16), DisplayName("确认密码"), Required]
        public string password2 { get; set; }
        [DisplayName("状态")]
        public int state { get ; set ; }
        [StringLength(2), DisplayName("性别")]
        public string gender { get; set; }
        [DisplayName("职务")]
        public int? postId { get; set; }
        [StringLength(20), DisplayName("办公电话")]
        public string officePhone { get; set; }
        [StringLength(20), DisplayName("相片名称")]
        public string picture { get; set; }
        [DisplayName("所属部门")]
        public int? deptId { get; set; }
        [DisplayName("所属科室")]
        public int? deptChild { get; set; }
        [DisplayName("角色")]
        public int? roleId { get; set; }
        public void toUserInfoDB(User_Info model)
        {
            model.real_name = PageValidate.InputText(realName, 50);
            model.user_certificate_no = PageValidate.InputText(certificateNo, 20);
            model.user_certificate_type = PageValidate.InputText(certificateType, 50);
            model.user_email = PageValidate.InputText(email, 100);
            model.user_mobile = PageValidate.InputText(mobile, 20);
            model.user_name = PageValidate.InputText(name, 20);
            model.user_state = state;
        }
        public void toUserExtendDB(User_Extend model)
        {
            model.user_dept_id = deptId==null?0:(deptChild==null?(int)deptId: (int)deptChild);
            model.user_gender = PageValidate.InputText(gender, 2);
            model.user_office_phone = officePhone!=null?PageValidate.InputText(officePhone, 20):"";
            model.user_picture = picture!=null?PageValidate.InputText(picture, 20):"";
            model.user_post_id = postId==null?0:(int)postId;
        }
    }
}