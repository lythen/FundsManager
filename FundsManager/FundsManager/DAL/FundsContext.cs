using System.Data.Entity;
using FundsManager.Models;
namespace FundsManager.DAL
{
    public class FundsContext: DbContext
    {
        public FundsContext() : base("FundsDBContext")
        {
        }
        public DbSet<Dic_Apply_State> Dic_Apply_State { get; set; }
        public DbSet<Dic_Department> Dic_Department { get; set; }
        public DbSet<Dic_Log_Type> Dic_Log_Type { get; set; }
        public DbSet<Dic_Post> Dic_Post { get; set; }
        public DbSet<Dic_Respond_State> Dic_Respond_State { get; set; }
        public DbSet<Dic_Role> Dic_Role { get; set; }
        public DbSet<Funds> Funds { get; set; }
        public DbSet<Funds_Apply> Funds_Apply { get; set; }
        public DbSet<Funds_Apply_Child> Funds_Apply_Child { get; set; }
        public DbSet<Process_Original> Process_Original { get; set; }
        public DbSet<Process_Respond> Process_Respond { get; set; }
        public DbSet<Sys_Log> Sys_Log { get; set; }
        public DbSet<User_Extend> User_Extend { get; set; }
        public DbSet<User_Info> User_Info { get; set; }
        public DbSet<User_vs_Role> User_vs_Role { get; set; }
    }
}