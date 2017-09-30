namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CfpSessionNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CfpSessions", "Notes", c => c.String());
            AddColumn("dbo.CfpSessions", "Status", c => c.String());
            AddColumn("dbo.CfpSessions", "Decided", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CfpSessions", "Decided");
            DropColumn("dbo.CfpSessions", "Status");
            DropColumn("dbo.CfpSessions", "Notes");
        }
    }
}
