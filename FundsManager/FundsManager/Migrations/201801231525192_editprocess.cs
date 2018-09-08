namespace Lythen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editprocess : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Process_Info",
                c => new
                    {
                        process_id = c.Int(nullable: false, identity: true),
                        process_name = c.String(maxLength: 50),
                        process_user_id = c.Int(nullable: false),
                        process_create_time = c.Int(nullable: false),
                        process_funds = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.process_id);
            
            CreateTable(
                "dbo.Process_List",
                c => new
                    {
                        po_id = c.Int(nullable: false, identity: true),
                        po_process_id = c.Int(nullable: false),
                        po_user_id = c.Int(nullable: false),
                        po_number = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.po_id);
            
            DropTable("dbo.Process_Original");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Process_Original",
                c => new
                    {
                        po_id = c.Int(nullable: false, identity: true),
                        po_f_id = c.Int(nullable: false),
                        po_user_id = c.Int(nullable: false),
                        po_number = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.po_id);
            
            DropTable("dbo.Process_List");
            DropTable("dbo.Process_Info");
        }
    }
}
