namespace Lythen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add20180628_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sys_Authority", "auth_info", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sys_Authority", "auth_info");
        }
    }
}
