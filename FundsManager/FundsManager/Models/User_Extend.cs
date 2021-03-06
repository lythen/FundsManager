﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace Lythen.Models
{
    /// <summary>
    /// 人员副表
    /// </summary>
    public class User_Extend
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int user_id { get; set; }
        [StringLength(2)]
        public string user_gender { get; set; }
        public int user_post_id { get; set; }
        [StringLength(20)]
        public string user_office_phone { get; set; }
        [StringLength(50)]
        public string user_picture { get; set; }
        public int user_dept_id { get; set; }
        public DateTime? user_add_time { get; set; }
        public int? user_add_user { get; set; }
        public DateTime? user_edit_time { get; set; }
        public int? user_edit_user { get; set; }
    }
}