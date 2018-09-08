namespace Lythen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatepan : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Process_Respond", "pr_apply_number", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Process_Respond", "pr_apply_number", c => c.String(maxLength: 9));
        }
    }
}
