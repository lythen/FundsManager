using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FundsManager.Models
{
    public class Dic_CardType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ctype_id { get; set; }
        [Required]
        [StringLength(50)]
        public string ctype_name { get; set; }
    }
}