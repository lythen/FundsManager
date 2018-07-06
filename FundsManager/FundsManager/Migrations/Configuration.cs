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
            //    new Dic_Role() { role_name = "ϵͳ����Ա" },
            //    new Dic_Role() { role_name = "�����û�" },
            //    new Dic_Role() { role_name = "��ͨ�û�" });
            //context.User_vs_Role.AddOrUpdate(x => x.uvr_user_id,
            //    new User_vs_Role() { uvr_role_id = 1, uvr_user_id = 1 },
            //    new User_vs_Role() { uvr_role_id = 2, uvr_user_id = 2 },
            //    new User_vs_Role() { uvr_role_id = 2, uvr_user_id = 2 }
            //    );
            //context.Dic_Log_Type.AddOrUpdate(x => x.dlt_log_name,
            //    new Dic_Log_Type() { dlt_log_name = "�û���½" },
            //    new Dic_Log_Type() { dlt_log_name = "��ӻ�����û���Ϣ" },
            //    new Dic_Log_Type() { dlt_log_name = "����" },
            //    new Dic_Log_Type() { dlt_log_name = "�ύ����" },
            //    new Dic_Log_Type() { dlt_log_name = "ϵͳ����" },
            //    new Dic_Log_Type() { dlt_log_name = "����" }
            //    );
            //context.Dic_Apply_State.AddOrUpdate(x => x.das_state_name,
            //    new Dic_Apply_State() { das_state_id = 0, das_state_name = "δ�Ķ�" },
            //    new Dic_Apply_State() { das_state_id = 1, das_state_name = "���Ķ�δ����" },
            //    new Dic_Apply_State() { das_state_id = 2, das_state_name = "������ͨ��" },
            //    new Dic_Apply_State() { das_state_id = 3, das_state_name = "����ͨ��" });
            //context.Dic_Respond_State.AddOrUpdate(x => x.drs_state_name,
            //    new Dic_Respond_State() { drs_state_id = 0, drs_state_name = "δ����" },
            //    new Dic_Respond_State() { drs_state_id = 1, drs_state_name = "����ͨ��" },
            //    new Dic_Respond_State() { drs_state_id = 2, drs_state_name = "������ͨ��" });
            //string path = ConfigurationManager.AppSettings["cPath"];
            //DirectoryInfo dir = new DirectoryInfo(path);
            //string name;
            //int i = 1;
            //Role_vs_Authority rva; Sys_Authority auth;
            //foreach (FileInfo file in dir.GetFiles())
            //{
            //    if (!file.Name.Contains("Controller")) continue;
            //    name = file.Name.Replace("Controller.cs", "");
            //    auth = new Sys_Authority()
            //    {
            //        auth_name = name,
            //         auth_is_Controller=true
            //    };
            //    rva = new Role_vs_Authority()
            //    {
            //        rva_role_id = 1,
            //        rva_auth_id = i
            //    };
            //    i++;
            //    context.Sys_Authority.AddOrUpdate(x => x.auth_name, auth);
            //    context.Role_vs_Authority.AddOrUpdate(x => new { x.rva_role_id, x.rva_auth_id }, rva);
            //}
            //string[] auths = { "�û�����", "��Ӿ���", "���ѹ���", "��������", "��������", "�������", "ϵͳ����" };
            //foreach (string name2 in auths)
            //{
            //    auth = new Sys_Authority()
            //    {
            //        auth_name = name2,
            //        auth_is_Controller = true
            //    };
            //    rva = new Role_vs_Authority()
            //    {
            //        rva_role_id = 1,
            //        rva_auth_id = i
            //    };
            //    i++;
            //    context.Sys_Authority.AddOrUpdate(x => x.auth_name, auth);
            //    context.Role_vs_Authority.AddOrUpdate(x => new { x.rva_role_id, x.rva_auth_id }, rva);
            //}
            //context.Sys_SiteInfo.AddOrUpdate(x => x.site_name,
            //    new Sys_SiteInfo() { site_name = "ĳĳ����ϵͳ" }
            //    );
            //context.Dic_CardType.AddOrUpdate(x => x.ctype_name,
            //    new Dic_CardType { ctype_name = "�����������֤" },
            //    new Dic_CardType { ctype_name = "����" },
            //    new Dic_CardType { ctype_name = "̨�����֤" },
            //    new Dic_CardType { ctype_name = "������֤" },
            //    new Dic_CardType { ctype_name = "���ž������֤" },
            //    new Dic_CardType { ctype_name = "�۰ľ��������ڵ�ͨ��֤" },
            //    new Dic_CardType { ctype_name = "̨�����������½ͨ��֤" },
            //    new Dic_CardType { ctype_name = "������Ч֤��" });
            //context.Dic_Reimbursement_Content.AddOrUpdate(x => x.content_title,
            //    new Dic_Reimbursement_Content { content_title = "�칫��" },
            //    new Dic_Reimbursement_Content { content_title = "�ʵ��" },
            //    new Dic_Reimbursement_Content { content_title = "ӡˢ��" },
            //    new Dic_Reimbursement_Content { content_title = "��ͨ��" },
            //    new Dic_Reimbursement_Content { content_title = "�����" },
            //    new Dic_Reimbursement_Content { content_title = "��ѵ��" },
            //    new Dic_Reimbursement_Content { content_title = "���ʲ��÷�" },
            //    new Dic_Reimbursement_Content { content_title = "�д���" },
            //    new Dic_Reimbursement_Content { content_title = "�Լ���" },
            //    new Dic_Reimbursement_Content { content_title = "���Ϸ�" },
            //    new Dic_Reimbursement_Content { content_title = "�����" },
            //    new Dic_Reimbursement_Content { content_title = "�������Է�" },
            //    new Dic_Reimbursement_Content { content_title = "ר����ѯ��" },
            //    new Dic_Reimbursement_Content { content_title = "�����" },
            //    new Dic_Reimbursement_Content { content_title = "�豸ά�޷�" },
            //    new Dic_Reimbursement_Content { content_title = "�豸���÷�" },
            //    new Dic_Reimbursement_Content { content_title = "�������ɷ�" },
            //    new Dic_Reimbursement_Content { content_title = "����" });
        }

    }
}
