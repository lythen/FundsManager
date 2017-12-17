using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    public class User_vs_Role
    {
        [Key]
        public int uvr_user_id { get; set; }
        public int uvr_role_id { get; set; }
    }
}