using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lythen.Models
{
    public class Dic_Role
    {
        [Key,DisplayName("角色ID")]
        public int role_id { get; set; }
        [StringLength(20), Required,DisplayName("角色名称")]
        public string role_name { get; set; }
    }
}