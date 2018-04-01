namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnew2 : DbMigration
    {
        public override void Up()
        {
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
            
            AddColumn("dbo.Recycle_Funds", "f_process", c => c.Int());
            DropTable("dbo.Funds_Apply_Recycle");
        }
        
        public override void Down()
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
            
            DropColumn("dbo.Recycle_Funds", "f_process");
            DropTable("dbo.Recycle_Funds_Apply");
        }
    }
}
