namespace Lythen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnew : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Funds", "f_code", c => c.String(maxLength: 20));
            AddColumn("dbo.Funds_Apply_Child", "c_get", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Funds_Apply_Child", "c_get_info", c => c.String(maxLength: 2000));
            AddColumn("dbo.Process_Respond", "next", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Process_Respond", "next");
            DropColumn("dbo.Funds_Apply_Child", "c_get_info");
            DropColumn("dbo.Funds_Apply_Child", "c_get");
            DropColumn("dbo.Funds", "f_code");
        }
    }
}
