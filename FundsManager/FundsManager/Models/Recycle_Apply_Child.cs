using System.ComponentModel.DataAnnotations;

namespace FundsManager.Models
{
    public class Recycle_Apply_Child
    {
        private int _c_state = 0;
        [StringLength(9)]
        public string c_apply_number { get; set; }
        [StringLength(13), Key]
        public string c_child_number { get; set; }
        public int c_funds_id { get; set; }
        [Required, DataType(DataType.Currency)]
        public decimal c_amount { get; set; }
        public int c_state { get { return _c_state; } set { _c_state = value; } }
        [StringLength(2000)]
        public string c_apply_for { get; set; }
        [DataType(DataType.Currency)]
        public decimal? c_get { get; set; }
        [StringLength(2000)]
        public string c_get_info { get; set; }
    }
}