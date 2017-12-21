namespace FundsManager.Migrations
{
    using System.Data.Entity.Migrations;
    using Models;
    using Common;
    using System.Configuration;
    using System.IO;

    internal sealed class Configuration : DbMigrationsConfiguration<FundsManager.DAL.FundsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DAL.FundsContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            //context.User_Info.AddOrUpdate(x => x.user_name,
            //    new User_Info() { real_name = "kBZaCvYQ3KdixR35kvH2Kg==", user_email = "Nb9k+d761UsDNjhQiZAxRSQyUMIceoZCTPVRCLE7b+g=", user_name = "sysAdmin", user_password = "7FUcyWfksXpO0X56JPsB29SPY318ApBmhHlftU17UNU2eEZ47bpkpVxb9YPR0pIB", user_mobile = "HObPslJFiZUcP9go8418gQ==", user_state = 1, user_salt = "6E262F901B" },
            //    new User_Info() { real_name = "1czN7v5wLaZvYc/OxzRmVQ==", user_email = "Nb9k+d761UsDNjhQiZAxRSQyUMIceoZCTPVRCLE7b+g=", user_name = "master", user_password = "7FUcyWfksXpO0X56JPsB29SPY318ApBmhHlftU17UNU2eEZ47bpkpVxb9YPR0pIB", user_mobile = "HObPslJFiZUcP9go8418gQ==", user_state = 1, user_salt = "6E262F901B" },
            //    new User_Info() { real_name = "Jq3VE6Db3CcAByEKgTp5DA==", user_email = "Nb9k+d761UsDNjhQiZAxRSQyUMIceoZCTPVRCLE7b+g=", user_name = "teacher", user_password = "7FUcyWfksXpO0X56JPsB29SPY318ApBmhHlftU17UNU2eEZ47bpkpVxb9YPR0pIB", user_mobile = "HObPslJFiZUcP9go8418gQ==", user_state = 1, user_salt = "6E262F901B" }
            //    );
            //context.Dic_Role.AddOrUpdate(x => x.role_name,
            //    new Dic_Role() { role_name = "系统管理员" },
            //    new Dic_Role() { role_name = "批复用户" },
            //    new Dic_Role() { role_name = "普通用户" });
            //context.User_vs_Role.AddOrUpdate(x => x.uvr_user_id,
            //    new User_vs_Role() { uvr_role_id = 1, uvr_user_id = 1 },
            //    new User_vs_Role() { uvr_role_id = 2, uvr_user_id = 2 },
            //    new User_vs_Role() { uvr_role_id = 2, uvr_user_id = 2 }
            //    );
            //context.Dic_Log_Type.AddOrUpdate(x => x.dlt_log_name,
            //    new Dic_Log_Type() { dlt_log_name = "用户登陆" },
            //    new Dic_Log_Type() { dlt_log_name = "添加或更新用户信息" },
            //    new Dic_Log_Type() { dlt_log_name = "批复" },
            //    new Dic_Log_Type() { dlt_log_name = "提交申请" },
            //    new Dic_Log_Type() { dlt_log_name = "系统配置" },
            //    new Dic_Log_Type() { dlt_log_name = "其他" }
            //    );
            //context.Dic_Apply_State.AddOrUpdate(x => x.das_state_name,
            //    new Dic_Apply_State() { das_state_id = 0, das_state_name = "未阅读" },
            //    new Dic_Apply_State() { das_state_id = 1, das_state_name = "已阅读未批复" },
            //    new Dic_Apply_State() { das_state_id = 2, das_state_name = "批复不通过" },
            //    new Dic_Apply_State() { das_state_id = 3, das_state_name = "批复通过" });
            //context.Dic_Respond_State.AddOrUpdate(x => x.drs_state_name,
            //    new Dic_Respond_State() { drs_state_id = 0, drs_state_name = "未批复" },
            //    new Dic_Respond_State() { drs_state_id = 1, drs_state_name = "批复通过" },
            //    new Dic_Respond_State() { drs_state_id = 2, drs_state_name = "批复不通过" });
            //string path = ConfigurationManager.AppSettings["cPath"];
            //DirectoryInfo dir = new DirectoryInfo(path);
            //string name;
            //foreach (FileInfo file in dir.GetFiles())
            //{
            //    if (!file.Name.Contains("Controller")) continue;
            //    name = file.Name.Replace("Controller.cs", "");
            //    Sys_Controller c = new Sys_Controller()
            //    {
            //        controller_name = name
            //    };
            //    Role_vs_Controller rvc = new Role_vs_Controller()
            //    {
            //        rvc_role_id = 1,
            //        rvc_controller = name
            //    };
            //    context.Sys_Controller.AddOrUpdate(x => x.controller_name, c);
            //    context.Role_vs_Controller.AddOrUpdate(x => new { x.rvc_role_id, x.rvc_controller }, rvc);
            //}
            //context.Sys_SiteInfo.AddOrUpdate(x => x.site_name,
            //    new Sys_SiteInfo() { site_name = "某某管理系统" }
            //    );
            //context.Dic_CardType.AddOrUpdate(x => x.ctype_name,
            //    new Dic_CardType { ctype_name = "二代居民身份证" },
            //    new Dic_CardType { ctype_name = "护照" },
            //    new Dic_CardType { ctype_name = "台湾身份证" },
            //    new Dic_CardType { ctype_name = "香港身份证" },
            //    new Dic_CardType { ctype_name = "澳门居民身份证" },
            //    new Dic_CardType { ctype_name = "港澳居民来往内地通行证" },
            //    new Dic_CardType { ctype_name = "台湾居民来往大陆通行证" },
            //    new Dic_CardType { ctype_name = "其它有效证件" });
        }

    }
}
