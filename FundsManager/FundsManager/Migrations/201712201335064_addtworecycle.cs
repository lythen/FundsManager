namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtworecycle : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Funds_Recycle");
            DropTable("dbo.Funds_Apply_Recycle");
        }
    }
}
