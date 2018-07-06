namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bigChange0706 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dic_Reimbursement_Content",
                c => new
                    {
                        content_id = c.Int(nullable: false, identity: true),
                        content_title = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.content_id);
            
            CreateTable(
                "dbo.Reimbursements",
                c => new
                    {
                        reimbursement_code = c.String(nullable: false, maxLength: 9),
                        r_add_user_id = c.Int(nullable: false),
                        r_add_date = c.DateTime(nullable: false),
                        r_bill_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        r_bill_state = c.Int(nullable: false),
                        reimbursement_info = c.String(),
                        r_funds_id = c.Int(nullable: false),
                        r_fact_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.reimbursement_code);
            
            CreateTable(
                "dbo.Reimbursement_Attachment",
                c => new
                    {
                        attachment_id = c.Int(nullable: false, identity: true),
                        atta_reimbursement_code = c.String(maxLength: 9),
                        atta_detail_id = c.Int(nullable: false),
                        attachment_path = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.attachment_id);
            
            CreateTable(
                "dbo.Reimbursement_Content",
                c => new
                    {
                        content_id = c.Int(nullable: false, identity: true),
                        c_reimbursement_code = c.String(maxLength: 9),
                        c_funds_id = c.Int(nullable: false),
                        c_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        c_dic_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.content_id);
            
            CreateTable(
                "dbo.Reimbursement_Detail",
                c => new
                    {
                        detail_id = c.Int(nullable: false, identity: true),
                        detail_content_id = c.Int(nullable: false),
                        detail_info = c.String(maxLength: 200),
                        detail_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        detail_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.detail_id);
            
            AddColumn("dbo.Funds", "f_add_Time", c => c.DateTime(nullable: false));
            AddColumn("dbo.Process_Respond", "pr_reimbursement_code", c => c.String(maxLength: 20));
            DropColumn("dbo.Funds", "f_expireDate");
            DropColumn("dbo.Process_Respond", "pr_apply_number");
            DropTable("dbo.Funds_Apply");
            DropTable("dbo.Funds_Apply_Child");
            DropTable("dbo.Recycle_Funds_Apply");
            DropTable("dbo.Recycle_Funds");
            DropTable("dbo.Recycle_User");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Recycle_User",
                c => new
                    {
                        delete_id = c.Int(nullable: false, identity: true),
                        user_id = c.Int(nullable: false),
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
                        user_gender = c.String(maxLength: 2),
                        user_post_id = c.Int(nullable: false),
                        user_office_phone = c.String(maxLength: 20),
                        user_picture = c.String(maxLength: 50),
                        user_dept_id = c.Int(nullable: false),
                        user_add_time = c.DateTime(),
                        user_add_user = c.Int(),
                        user_edit_time = c.DateTime(),
                        user_edit_user = c.Int(),
                    })
                .PrimaryKey(t => t.delete_id);
            
            CreateTable(
                "dbo.Recycle_Funds",
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
                        f_delete_time = c.DateTime(nullable: false),
                        f_state = c.Int(nullable: false),
                        f_process = c.Int(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Recycle_Funds_Apply",
                c => new
                    {
                        apply_number = c.String(nullable: false, maxLength: 9),
                        apply_user_id = c.Int(nullable: false),
                        apply_time = c.DateTime(nullable: false),
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
                        c_funds_id = c.Int(nullable: false),
                        c_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        c_state = c.Int(nullable: false),
                        c_apply_for = c.String(maxLength: 2000),
                        c_get = c.Decimal(nullable: false, precision: 18, scale: 2),
                        c_get_info = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.c_child_number);
            
            CreateTable(
                "dbo.Funds_Apply",
                c => new
                    {
                        apply_number = c.String(nullable: false, maxLength: 9),
                        apply_user_id = c.Int(nullable: false),
                        apply_time = c.DateTime(nullable: false),
                        apply_amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        apply_state = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.apply_number);
            
            AddColumn("dbo.Process_Respond", "pr_apply_number", c => c.String(maxLength: 20));
            AddColumn("dbo.Funds", "f_expireDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Process_Respond", "pr_reimbursement_code");
            DropColumn("dbo.Funds", "f_add_Time");
            DropTable("dbo.Reimbursement_Detail");
            DropTable("dbo.Reimbursement_Content");
            DropTable("dbo.Reimbursement_Attachment");
            DropTable("dbo.Reimbursements");
            DropTable("dbo.Dic_Reimbursement_Content");
        }
    }
}
