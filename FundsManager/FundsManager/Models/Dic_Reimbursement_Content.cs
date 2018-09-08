using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lythen.Models
{
    /// <summary>
    /// 报销内容
    /// </summary>
    public class Dic_Reimbursement_Content
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int content_id { get; set; }
        [Required]
        [StringLength(50)]
        public string content_title { get; set; }
    }
}