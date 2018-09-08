namespace Lythen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recycle_Funds", "f_state", c => c.Int(nullable: false));
            AlterColumn("dbo.Recycle_Funds", "f_delete_time", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Recycle_Funds", "f_delete_time", c => c.Int(nullable: false));
            DropColumn("dbo.Recycle_Funds", "f_state");
        }
    }
}
