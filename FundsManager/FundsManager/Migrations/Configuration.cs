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
                new User_Info() { real_name = "ϵͳ����Ա", user_email = "lyrenlian@163.com", user_name = "sysAdmin", user_password = psw, user_mobile = "13632393905", user_state = 1, user_salt = "6E262F901B" },
                new User_Info() { real_name = "�쵼1", user_email = "lyrenlian@163.com", user_name = "master", user_password = psw, user_mobile = "13632393905", user_state = 1, user_salt = "6E262F901B" },
                new User_Info() { real_name = "�û�1", user_email = "lyrenlian@163.com", user_name = "teacher", user_password = psw, user_mobile = "13632393905", user_state = 1, user_salt = "6E262F901B" }
                );
            context.Dic_Role.AddOrUpdate(x => x.role_name,
                new Dic_Role() { role_name = "ϵͳ����Ա" },
                new Dic_Role() { role_name = "�����û�" },
                new Dic_Role() { role_name = "��ͨ�û�" });
            context.User_vs_Role.AddOrUpdate(x => x.uvr_user_id,
                new User_vs_Role() { uvr_role_id = 1, uvr_user_id = 1 },
                new User_vs_Role() { uvr_role_id = 2, uvr_user_id = 2 },
                new User_vs_Role() { uvr_role_id = 2, uvr_user_id = 2 }
                );
            context.Dic_Log_Type.AddOrUpdate(x => x.dlt_log_name,
                new Dic_Log_Type() { dlt_log_name = "�û���½" },
                new Dic_Log_Type() { dlt_log_name = "��ӻ�����û���Ϣ" },
                new Dic_Log_Type() { dlt_log_name = "����" },
                new Dic_Log_Type() { dlt_log_name = "�ύ����" },
                new Dic_Log_Type() { dlt_log_name = "ϵͳ����" },
                new Dic_Log_Type() { dlt_log_name = "����" }
                );
            context.Dic_Apply_State.AddOrUpdate(x => x.das_state_name,
                new Dic_Apply_State() { das_state_name = "δ�Ķ�" },
                new Dic_Apply_State() { das_state_name = "���Ķ�δ����" },
                new Dic_Apply_State() { das_state_name = "������ͨ��" },
                new Dic_Apply_State() { das_state_name = "����ͨ��" });
            context.Dic_Respond_State.AddOrUpdate(x => x.drs_state_name,
                new Dic_Respond_State() { drs_state_name = "δ����" },
                new Dic_Respond_State() { drs_state_name = "����ͨ��" },
                new Dic_Respond_State() { drs_state_name = "������ͨ��" });
        }
    }
}
