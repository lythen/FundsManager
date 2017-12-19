namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using FundsManager.Models;
    using FundsManager.Common;
    internal sealed class Configuration : DbMigrationsConfiguration<FundsManager.DAL.FundsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FundsManager.DAL.FundsContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            string psw = PasswordUnit.getPassword("63396a3725a910f5d0caa43e9a2b08ac", "6E262F901B");
            
            context.User_Info.AddOrUpdate(x => x.user_name,
                new User_Info() { real_name = "系统管理员", user_email = "lyrenlian@163.com", user_name = "sysAdmin", user_password = psw, user_mobile = "13632393905", user_state = 1, user_salt = "6E262F901B" },
                new User_Info() { real_name = "领导1", user_email = "lyrenlian@163.com", user_name = "master", user_password = psw, user_mobile = "13632393905", user_state = 1, user_salt = "6E262F901B" },
                new User_Info() { real_name = "用户1", user_email = "lyrenlian@163.com", user_name = "teacher", user_password = psw, user_mobile = "13632393905", user_state = 1, user_salt = "6E262F901B" }
                );
            context.Dic_Role.AddOrUpdate(x => x.role_name,
                new Dic_Role() { role_name = "系统管理员" },
                new Dic_Role() { role_name = "批复用户" },
                new Dic_Role() { role_name = "普通用户" });
            context.User_vs_Role.AddOrUpdate(x => x.uvr_user_id,
                new User_vs_Role() { uvr_role_id = 1, uvr_user_id = 1 },
                new User_vs_Role() { uvr_role_id = 2, uvr_user_id = 2 },
                new User_vs_Role() { uvr_role_id = 2, uvr_user_id = 2 }
                );
            context.Dic_Log_Type.AddOrUpdate(x => x.dlt_log_name,
                new Dic_Log_Type() { dlt_log_name = "用户登陆" },
                new Dic_Log_Type() { dlt_log_name = "添加或更新用户信息" },
                new Dic_Log_Type() { dlt_log_name = "批复" },
                new Dic_Log_Type() { dlt_log_name = "提交申请" },
                new Dic_Log_Type() { dlt_log_name = "系统配置" },
                new Dic_Log_Type() { dlt_log_name = "其他" }
                );
            context.Dic_Apply_State.AddOrUpdate(x => x.das_state_name,
                new Dic_Apply_State() { das_state_name = "未阅读" },
                new Dic_Apply_State() { das_state_name = "已阅读未批复" },
                new Dic_Apply_State() { das_state_name = "批复不通过" },
                new Dic_Apply_State() { das_state_name = "批复通过" });
            context.Dic_Respond_State.AddOrUpdate(x => x.drs_state_name,
                new Dic_Respond_State() { drs_state_name = "未批复" },
                new Dic_Respond_State() { drs_state_name = "批复通过" },
                new Dic_Respond_State() { drs_state_name = "批复不通过" });
        }
    }
}
