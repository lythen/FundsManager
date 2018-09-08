using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lythen.Models
{
    public class Dic_Post
    {
        [Key, DisplayName("职务ID")]
        public int post_id { get; set; }
        [StringLength(20), Required,DisplayName("职务名称")]
        public string post_name { get; set; }
    }
}