namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateLog : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sys_Log", "log_target", c => c.String(maxLength: 100));
            AlterColumn("dbo.Sys_Log", "log_content", c => c.String(maxLength: 2000));
            AlterColumn("dbo.Sys_Log", "log_ip", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sys_Log", "log_ip", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Sys_Log", "log_content", c => c.String(nullable: false, maxLength: 2000));
            AlterColumn("dbo.Sys_Log", "log_target", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
