﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lythen.Models
{
    /// <summary>
    /// 批复流程列表 -废弃，不用！
    /// </summary>
    public class Process_List
    {
        [Key]
        public int po_id { get; set; }
        public int po_process_id { get; set; }
        [DisplayName("批复领导")]
        public int po_user_id { get; set; }
        public int po_sort { get; set; }
    }
}