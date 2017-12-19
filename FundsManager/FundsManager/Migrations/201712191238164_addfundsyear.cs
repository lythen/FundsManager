namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfundsyear : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Funds", "f_in_year", c => c.String(maxLength: 4));
            AddColumn("dbo.Funds", "f_expireDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Funds", "f_expireDate");
            DropColumn("dbo.Funds", "f_in_year");
        }
    }
}
