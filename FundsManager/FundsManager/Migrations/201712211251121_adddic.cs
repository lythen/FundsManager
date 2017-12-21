namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddic : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dic_CardType",
                c => new
                    {
                        ctype_id = c.Int(nullable: false, identity: true),
                        ctype_name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ctype_id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Dic_CardType");
        }
    }
}
