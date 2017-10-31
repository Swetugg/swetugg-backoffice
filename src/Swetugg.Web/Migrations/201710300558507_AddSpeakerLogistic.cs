namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSpeakerLogistic : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SpeakerLogistics",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Notes = c.String(),
                        AttendingDinner = c.Boolean(nullable: false),
                        TwitterList = c.Boolean(nullable: false),
                        AccomodationDone = c.Boolean(nullable: false),
                        TravelDone = c.Boolean(nullable: false),
                        SpeakerId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Speakers", t => t.SpeakerId)
                .Index(t => t.SpeakerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SpeakerLogistics", "SpeakerId", "dbo.Speakers");
            DropIndex("dbo.SpeakerLogistics", new[] { "SpeakerId" });
            DropTable("dbo.SpeakerLogistics");
        }
    }
}
