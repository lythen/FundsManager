using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    public class Sys_Controller
    {
        [Key]
        public int id { get; set; }
        public string controller_name { get; set; }
    }
}