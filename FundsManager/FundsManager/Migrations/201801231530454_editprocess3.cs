namespace Lythen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editprocess3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Process_Info", "process_create_time", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Process_Info", "process_create_time", c => c.Int(nullable: false));
        }
    }
}
