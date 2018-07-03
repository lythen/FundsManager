namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add20180628_1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Role_vs_Authority",
                c => new
                    {
                        rva_role_id = c.Int(nullable: false),
                        rva_auth_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.rva_role_id, t.rva_auth_id });
            
            CreateTable(
                "dbo.Sys_Authority",
                c => new
                    {
                        auth_id = c.Int(nullable: false, identity: true),
                        auth_name = c.String(nullable: false, maxLength: 20),
                        auth_is_Controller = c.Boolean(nullable: false),
                        auth_map_Controller = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.auth_id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sys_Authority");
            DropTable("dbo.Role_vs_Authority");
        }
    }
}
