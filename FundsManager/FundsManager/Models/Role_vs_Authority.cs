using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lythen.Models
{
    /// <summary>
    /// 角色与权限对应表
    /// </summary>
    public class Role_vs_Authority
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column(Order = 1)]
        public int rva_role_id { get; set; }
        [Key, Column(Order = 2)]
        public int rva_auth_id { get; set; }
    }
}