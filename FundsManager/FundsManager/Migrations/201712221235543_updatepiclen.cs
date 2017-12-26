namespace FundsManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatepiclen : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.User_Extend", "user_picture", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.User_Extend", "user_picture", c => c.String(maxLength: 20));
        }
    }
}
