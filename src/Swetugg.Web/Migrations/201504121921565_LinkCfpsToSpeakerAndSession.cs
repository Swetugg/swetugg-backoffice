namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkCfpsToSpeakerAndSession : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CfpSessions", "SessionId", c => c.Int());
            AddColumn("dbo.CfpSpeakers", "SpeakerId", c => c.Int());
            CreateIndex("dbo.CfpSessions", "SessionId");
            CreateIndex("dbo.CfpSpeakers", "SpeakerId");
            AddForeignKey("dbo.CfpSessions", "SessionId", "dbo.Sessions", "Id");
            AddForeignKey("dbo.CfpSpeakers", "SpeakerId", "dbo.Speakers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CfpSpeakers", "SpeakerId", "dbo.Speakers");
            DropForeignKey("dbo.CfpSessions", "SessionId", "dbo.Sessions");
            DropIndex("dbo.CfpSpeakers", new[] { "SpeakerId" });
            DropIndex("dbo.CfpSessions", new[] { "SessionId" });
            DropColumn("dbo.CfpSpeakers", "SpeakerId");
            DropColumn("dbo.CfpSessions", "SessionId");
        }
    }
}
