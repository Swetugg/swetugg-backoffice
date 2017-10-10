namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CfpSessionCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CfpSessions", "Category", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CfpSessions", "Category");
        }
    }
}
