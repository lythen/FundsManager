namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcontrollerinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sys_Controller", "controller_info", c => c.String(maxLength: 1000));
            AlterColumn("dbo.Sys_Controller", "controller_name", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sys_Controller", "controller_name", c => c.String());
            DropColumn("dbo.Sys_Controller", "controller_info");
        }
    }
}
