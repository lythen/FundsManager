using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    public class Sys_Controller
    {
        [Key]
        public int id { get; set; }
        [StringLength(20)]
        public string controller_name { get; set; }
        [StringLength(1000)]
        public string controller_info { get; set; }
    }
}