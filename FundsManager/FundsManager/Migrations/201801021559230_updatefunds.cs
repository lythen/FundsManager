namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatefunds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Funds", "f_state", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Funds", "f_state");
        }
    }
}
