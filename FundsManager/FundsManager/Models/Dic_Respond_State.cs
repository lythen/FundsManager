using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    public class Dic_Respond_State
    {
        [Key]
        public int drs_state_id { get; set; }
        [StringLength(20), Required]
        public string drs_state_name { get; set; }
    }
}