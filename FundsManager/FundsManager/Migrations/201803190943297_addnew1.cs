namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnew1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Funds_Apply_Child", "c_get", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Funds_Apply_Child", "c_get", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
