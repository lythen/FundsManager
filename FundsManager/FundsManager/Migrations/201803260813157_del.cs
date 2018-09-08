namespace Lythen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class del : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Dic_Apply_State");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Dic_Apply_State",
                c => new
                    {
                        das_state_id = c.Int(nullable: false),
                        das_state_name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.das_state_id);
            
        }
    }
}
