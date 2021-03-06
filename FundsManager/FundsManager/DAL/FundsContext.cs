﻿using System.Data.Entity;
using Lythen.Models;
using System.Linq;
namespace Lythen.DAL
{
    public class LythenContext: DbContext
    {
        public LythenContext() : base("FundsDBContext")
        {
        }
        public DbSet<Dic_Department> Dic_Department { get; set; }
        public DbSet<Dic_Log_Type> Dic_Log_Type { get; set; }
        public DbSet<Dic_Post> Dic_Post { get; set; }
        public DbSet<Dic_Respond_State> Dic_Respond_State { get; set; }
        public DbSet<Dic_Role> Dic_Role { get; set; }
        public DbSet<Funds> Funds { get; set; }
        public DbSet<Reimbursement> Reimbursement { get; set; }
        public DbSet<Reimbursement_Content> Reimbursement_Content { get; set; }
        public DbSet<Process_List> Process_List { get; set; }
        public DbSet<Process_Respond> Process_Respond { get; set; }
        public DbSet<Sys_Log> Sys_Log { get; set; }
        public DbSet<User_Extend> User_Extend { get; set; }
        public DbSet<User_Info> User_Info { get; set; }
        public DbSet<User_vs_Role> User_vs_Role { get; set; }
        public DbSet<Role_vs_Controller> Role_vs_Controller{ get; set; }
        public DbSet<Sys_Controller> Sys_Controller { get; set; }
        public DbSet<Sys_SiteInfo> Sys_SiteInfo { get; set; }
        public DbSet<Dic_CardType> Dic_CardType { get; set; }
        public DbSet<Process_Info> Process_Info { get; set; }
        public DbSet<Sys_Authority> Sys_Authority { get; set; }
        public DbSet<Role_vs_Authority> Role_vs_Authority { get; set; }
        public DbSet<Dic_Reimbursement_Content> Dic_Reimbursement_Content { get; set; }
        public DbSet<Reimbursement_Attachment> Reimbursement_Attachment { get; set; }
        public DbSet<Reimbursement_Detail> Reimbursement_Detail { get; set; }

    }
}