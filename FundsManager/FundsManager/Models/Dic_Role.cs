using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    public class Dic_Role
    {
        [Key]
        public int role_id { get; set; }
        [StringLength(20), Required]
        public string role_name { get; set; }
    }
}