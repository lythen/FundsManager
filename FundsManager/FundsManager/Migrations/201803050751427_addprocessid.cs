namespace Lythen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addprocessid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Funds", "f_process", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Funds", "f_process");
        }
    }
}
