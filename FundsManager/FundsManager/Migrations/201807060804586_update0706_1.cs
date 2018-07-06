namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update0706_1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Funds", "f_in_year");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Funds", "f_in_year", c => c.String(maxLength: 4));
        }
    }
}
