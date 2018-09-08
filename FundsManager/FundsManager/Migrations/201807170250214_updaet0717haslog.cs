namespace Lythen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updaet0717haslog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reimbursements", "c_has_log", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reimbursements", "c_has_log");
        }
    }
}
