namespace Lythen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewdb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dic_Apply_State",
                c => new
                    {
                        das_state_id = c.Int(nullable: false),
                        das_state_name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.das_state_id);
            
            CreateTable(
                "dbo.Dic_CardType",
                c => new
                    {
                        ctype_id = c.Int(nullable: false, identity: true),
                        ctype_name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ctype_id);
            
            CreateTable(
                "dbo.Dic_Department",
                c => new
                    {
                        dept_id = c.Int(nullable: false, identity: true),
                        dept_name = c.String(nullable: false, maxLength: 20),
                        dept_parent_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.dept_id);
            
            CreateTable(
                "dbo.Dic_Log_Type",
                c => new
                    {
                        dlt_log_id = c.Int(nullable: false, identity: true),
                        dlt_log_name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.dlt_log_id);
            
            CreateTable(
                "dbo.Dic_Post",
                c => new
                    {
                        post_id = c.Int(nullable: false, identity: true),
                        post_name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.post_id);
            
            CreateTable(
                "dbo.Dic_Respond_State",
                c => new
                    {
                        drs_state_id = c.Int(nullable: false),
                        drs_state_name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.drs_state_id);
            
            CreateTable(
                "dbo.Dic_Role",
                c => new
                    {
                        role_id = c.Int(nullable: false, identity: true),
                        role_name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.role_id);
            
            CreateTable(
                "dbo.Funds",
                c => new
                    {
                        f_id = c.Int(nullable: false, identity: true),
                        f_name = c.String(maxLength: 100),
                        f_in_year = c.String(maxLength: 4),
                        f_expireDate = c.DateTime(nullable: false),
                        f_source = c.String(maxLength: 100),
                        f_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        f_balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        f_manager = c.Int(nullable: false),
                        f_info = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.f_id);
            
            CreateTable(
                "dbo.Funds_Apply",
                c => new
                    {
                        apply_number = c.String(nullable: false, maxLength: 9),
                        apply_user_id = c.Int(nullable: false),
                        apply_time = c.DateTime(nullable: false),
                        apply_funds_id = c.Int(nullable: false),
                        apply_for = c.String(maxLength: 2000),
                        apply_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        apply_state = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.apply_number);
            
            CreateTable(
                "dbo.Funds_Apply_Child",
                c => new
                    {
                        c_child_number = c.String(nullable: false, maxLength: 13),
                        c_apply_number = c.String(maxLength: 9),
                        c_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        c_state = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.c_child_number);
            
            CreateTable(
                "dbo.Funds_Apply_Recycle",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        apply_number = c.String(maxLength: 9),
                        apply_user_id = c.Int(nullable: false),
                        apply_time = c.DateTime(nullable: false),
                        apply_funds_id = c.Int(nullable: false),
                        apply_for = c.String(maxLength: 2000),
                        apply_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        apply_state = c.Int(nullable: false),
                        apply_delete_user = c.Int(nullable: false),
                        apply_delete_time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Funds_Recycle",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        f_id = c.Int(nullable: false),
                        f_name = c.String(maxLength: 100),
                        f_in_year = c.String(maxLength: 4),
                        f_expireDate = c.DateTime(nullable: false),
                        f_source = c.String(maxLength: 100),
                        f_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        f_balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        f_manager = c.Int(nullable: false),
                        f_info = c.String(maxLength: 2000),
                        f_delete_user = c.Int(nullable: false),
                        f_delete_time = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Process_Original",
                c => new
                    {
                        po_id = c.Int(nullable: false, identity: true),
                        po_f_id = c.Int(nullable: false),
                        po_user_id = c.Int(nullable: false),
                        po_number = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.po_id);
            
            CreateTable(
                "dbo.Process_Respond",
                c => new
                    {
                        pr_id = c.Int(nullable: false, identity: true),
                        pr_apply_number = c.String(maxLength: 9),
                        pr_user_id = c.Int(nullable: false),
                        pr_number = c.Int(nullable: false),
                        pr_time = c.DateTime(nullable: false),
                        pr_content = c.String(maxLength: 2000),
                        pr_state = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.pr_id);
            
            CreateTable(
                "dbo.Role_vs_Controller",
                c => new
                    {
                        rvc_role_id = c.Int(nullable: false),
                        rvc_controller = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.rvc_role_id, t.rvc_controller });
            
            CreateTable(
                "dbo.Sys_Controller",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        controller_name = c.String(maxLength: 20),
                        controller_info = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Sys_Log",
                c => new
                    {
                        log_id = c.Int(nullable: false, identity: true),
                        log_user_id = c.Int(nullable: false),
                        log_target = c.String(nullable: false, maxLength: 100),
                        log_content = c.String(nullable: false, maxLength: 2000),
                        log_type = c.Int(nullable: false),
                        log_time = c.DateTime(nullable: false),
                        log_ip = c.String(nullable: false, maxLength: 150),
                        log_device = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.log_id);
            
            CreateTable(
                "dbo.Sys_SiteInfo",
                c => new
                    {
                        site_name = c.String(nullable: false, maxLength: 100),
                        site_company = c.String(maxLength: 100),
                        site_introduce = c.String(maxLength: 2000),
                        site_company_address = c.String(maxLength: 200),
                        site_company_phone = c.String(maxLength: 20),
                        site_company_email = c.String(maxLength: 100),
                        site_manager_name = c.String(maxLength: 50),
                        site_manager_phone = c.String(maxLength: 20),
                        site_manager_email = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.site_name);
            
            CreateTable(
                "dbo.User_Extend",
                c => new
                    {
                        user_id = c.Int(nullable: false),
                        user_gender = c.String(maxLength: 2),
                        user_post_id = c.Int(nullable: false),
                        user_office_phone = c.String(maxLength: 20),
                        user_picture = c.String(maxLength: 50),
                        user_dept_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.user_id);
            
            CreateTable(
                "dbo.User_Info",
                c => new
                    {
                        user_id = c.Int(nullable: false, identity: true),
                        user_name = c.String(nullable: false, maxLength: 20),
                        real_name = c.String(maxLength: 100),
                        user_certificate_type = c.String(maxLength: 50),
                        user_certificate_no = c.String(maxLength: 100),
                        user_mobile = c.String(maxLength: 100),
                        user_email = c.String(maxLength: 200),
                        user_password = c.String(maxLength: 200),
                        user_salt = c.String(maxLength: 10),
                        user_state = c.Int(nullable: false),
                        user_login_times = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.user_id);
            
            CreateTable(
                "dbo.User_vs_Role",
                c => new
                    {
                        uvr_user_id = c.Int(nullable: false, identity: true),
                        uvr_role_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.uvr_user_id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.User_vs_Role");
            DropTable("dbo.User_Info");
            DropTable("dbo.User_Extend");
            DropTable("dbo.Sys_SiteInfo");
            DropTable("dbo.Sys_Log");
            DropTable("dbo.Sys_Controller");
            DropTable("dbo.Role_vs_Controller");
            DropTable("dbo.Process_Respond");
            DropTable("dbo.Process_Original");
            DropTable("dbo.Funds_Recycle");
            DropTable("dbo.Funds_Apply_Recycle");
            DropTable("dbo.Funds_Apply_Child");
            DropTable("dbo.Funds_Apply");
            DropTable("dbo.Funds");
            DropTable("dbo.Dic_Role");
            DropTable("dbo.Dic_Respond_State");
            DropTable("dbo.Dic_Post");
            DropTable("dbo.Dic_Log_Type");
            DropTable("dbo.Dic_Department");
            DropTable("dbo.Dic_CardType");
            DropTable("dbo.Dic_Apply_State");
        }
    }
}
