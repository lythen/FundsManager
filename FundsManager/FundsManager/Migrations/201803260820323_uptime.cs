namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uptime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Process_Respond", "pr_time", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Process_Respond", "pr_time", c => c.DateTime(nullable: false));
        }
    }
}
