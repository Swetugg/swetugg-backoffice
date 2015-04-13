namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreRequiredFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CfpSessions", "Tags", c => c.String(maxLength: 250));
            AlterColumn("dbo.CfpSessions", "Audience", c => c.String(maxLength: 250));
            AlterColumn("dbo.CfpSessions", "Level", c => c.String(maxLength: 250));
            AlterColumn("dbo.CfpSpeakers", "Email", c => c.String(nullable: false, maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CfpSpeakers", "Email", c => c.String(maxLength: 250));
            AlterColumn("dbo.CfpSessions", "Level", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.CfpSessions", "Audience", c => c.String(nullable: false, maxLength: 250));
            AlterColumn("dbo.CfpSessions", "Tags", c => c.String(nullable: false, maxLength: 250));
        }
    }
}
