namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatefa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Funds_Apply_Child", "c_funds_id", c => c.Int(nullable: false));
            AddColumn("dbo.Funds_Apply_Child", "c_apply_for", c => c.String(maxLength: 2000));
            DropColumn("dbo.Funds_Apply", "apply_funds_id");
            DropColumn("dbo.Funds_Apply", "apply_for");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Funds_Apply", "apply_for", c => c.String(maxLength: 2000));
            AddColumn("dbo.Funds_Apply", "apply_funds_id", c => c.Int(nullable: false));
            DropColumn("dbo.Funds_Apply_Child", "c_apply_for");
            DropColumn("dbo.Funds_Apply_Child", "c_funds_id");
        }
    }
}
