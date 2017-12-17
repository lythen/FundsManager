namespace FundsManager.ViewModels
{
    public class UserModel
    {
        public int? id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 上一次登陆时间
        /// </summary>
        public string lastTime { get; set; }
        /// <summary>
        /// 上一次登陆IP
        /// </summary>
        public string lastIp { get; set; }
        /// <summary>
        /// 上一次登陆设备
        /// </summary>
        public string lastDev { get; set; }
        /// <summary>
        /// 登陆次数
        /// </summary>
        public int times { get; set; }
        /// <summary>
        /// 登陆角色
        /// </summary>
        public string roleName { get; set; }
    }
}