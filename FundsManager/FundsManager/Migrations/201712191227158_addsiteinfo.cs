namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsiteinfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sys_SiteInfo",
                c => new
                    {
                        site_name = c.String(nullable: false, maxLength: 100),
                        site_company = c.String(maxLength: 100),
                        site_introduce = c.String(maxLength: 2000),
                        site_company_address = c.String(maxLength: 200),
                        site_company_phone = c.String(maxLength: 20),
                        site_company_email = c.String(maxLength: 100),
                        site_manager_name = c.String(maxLength: 50),
                        site_manater_phone = c.String(maxLength: 20),
                        site_email = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.site_name);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sys_SiteInfo");
        }
    }
}
