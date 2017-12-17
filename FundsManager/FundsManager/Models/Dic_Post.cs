using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    public class Dic_Post
    {
        [Key]
        public int post_id { get; set; }
        [StringLength(20), Required]
        public string post_name { get; set; }
    }
}