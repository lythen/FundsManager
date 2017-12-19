namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addeditsiteinfotable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sys_SiteInfo", "site_manager_phone", c => c.String(maxLength: 20));
            AddColumn("dbo.Sys_SiteInfo", "site_manager_email", c => c.String(maxLength: 100));
            DropColumn("dbo.Sys_SiteInfo", "site_manater_phone");
            DropColumn("dbo.Sys_SiteInfo", "site_email");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sys_SiteInfo", "site_email", c => c.String(maxLength: 100));
            AddColumn("dbo.Sys_SiteInfo", "site_manater_phone", c => c.String(maxLength: 20));
            DropColumn("dbo.Sys_SiteInfo", "site_manager_email");
            DropColumn("dbo.Sys_SiteInfo", "site_manager_phone");
        }
    }
}
