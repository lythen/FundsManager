namespace Lythen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addrecy : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Funds_Recycle", newName: "Recycle_Funds");
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
            
            AddColumn("dbo.User_Extend", "user_add_time", c => c.DateTime());
            AddColumn("dbo.User_Extend", "user_add_user", c => c.Int());
            AddColumn("dbo.User_Extend", "user_edit_time", c => c.DateTime());
            AddColumn("dbo.User_Extend", "user_edit_user", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.User_Extend", "user_edit_user");
            DropColumn("dbo.User_Extend", "user_edit_time");
            DropColumn("dbo.User_Extend", "user_add_user");
            DropColumn("dbo.User_Extend", "user_add_time");
            DropTable("dbo.Recycle_User");
            RenameTable(name: "dbo.Recycle_Funds", newName: "Funds_Recycle");
        }
    }
}
