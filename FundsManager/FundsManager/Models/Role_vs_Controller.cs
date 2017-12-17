using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FundsManager.Models
{
    public class Role_vs_Controller
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None),Column(Order = 1)]
        public int rvc_role_id { get; set; }
        [Key, Column(Order=2)]
        public string rvc_controller { get; set; }
    }
}