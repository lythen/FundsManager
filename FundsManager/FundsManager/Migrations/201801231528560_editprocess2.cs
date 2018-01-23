namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editprocess2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Process_List", "po_sort", c => c.Int(nullable: false));
            DropColumn("dbo.Process_List", "po_number");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Process_List", "po_number", c => c.Int(nullable: false));
            DropColumn("dbo.Process_List", "po_sort");
        }
    }
}
