using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lythen.Models
{
    /// <summary>
    /// 权限集合
    /// </summary>
    public class Sys_Authority
    {
        private bool _auth_is_Controller = false;
        private int _auth_map_Controller = 0;
        [Key, DisplayName("权限ID")]
        public int auth_id { get; set; }
        [StringLength(20), Required, DisplayName("权限名称")]
        public string auth_name { get; set; }
        [Required, DisplayName("权限是否是控制器")]
        public bool auth_is_Controller { get { return _auth_is_Controller; } set { _auth_is_Controller = value; } }
        [DisplayName("权限对应的板块")]
        public int auth_map_Controller { get { return _auth_map_Controller; } set { _auth_map_Controller = value; } }
        [StringLength(2000), DisplayName("权限说明")]
        public string auth_info { get; set; }
    }
}