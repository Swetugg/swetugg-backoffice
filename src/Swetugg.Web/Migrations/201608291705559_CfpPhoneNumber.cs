namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CfpPhoneNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CfpSpeakers", "Phone", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CfpSpeakers", "Phone");
        }
    }
}
