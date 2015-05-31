namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpeakerImages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SpeakerImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SpeakerId = c.Int(nullable: false),
                        ImageTypeId = c.Int(nullable: false),
                        ImageUrl = c.String(nullable: false, maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ImageTypes", t => t.ImageTypeId)
                .ForeignKey("dbo.Speakers", t => t.SpeakerId)
                .Index(t => t.SpeakerId)
                .Index(t => t.ImageTypeId);
            
            CreateTable(
                "dbo.ImageTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConferenceId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        Slug = c.String(nullable: false, maxLength: 250),
                        Width = c.Int(),
                        Height = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Conferences", t => t.ConferenceId)
                .Index(t => t.ConferenceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ImageTypes", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.SpeakerImages", "SpeakerId", "dbo.Speakers");
            DropForeignKey("dbo.SpeakerImages", "ImageTypeId", "dbo.ImageTypes");
            DropIndex("dbo.ImageTypes", new[] { "ConferenceId" });
            DropIndex("dbo.SpeakerImages", new[] { "ImageTypeId" });
            DropIndex("dbo.SpeakerImages", new[] { "SpeakerId" });
            DropTable("dbo.ImageTypes");
            DropTable("dbo.SpeakerImages");
        }
    }
}
