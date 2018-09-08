using System.ComponentModel.DataAnnotations;

namespace Lythen.Models
{
    /// <summary>
    /// 部门表
    /// </summary>
    public class Dic_Department
    {
        private int _dept_parent_id = 0;
        [Key]
        public int dept_id { get; set; }
        [StringLength(20), Required]
        public string dept_name { get; set; }
        public int dept_parent_id { get { return _dept_parent_id; }set { _dept_parent_id = value; } }
    }
}