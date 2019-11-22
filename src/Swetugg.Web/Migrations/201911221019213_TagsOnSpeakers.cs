namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TagsOnSpeakers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SpeakerTags",
                c => new
                    {
                        Speaker_Id = c.Int(nullable: false),
                        Tag_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Speaker_Id, t.Tag_Id })
                .ForeignKey("dbo.Speakers", t => t.Speaker_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .Index(t => t.Speaker_Id)
                .Index(t => t.Tag_Id);
            
            AddColumn("dbo.Tags", "Type", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SpeakerTags", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.SpeakerTags", "Speaker_Id", "dbo.Speakers");
            DropIndex("dbo.SpeakerTags", new[] { "Tag_Id" });
            DropIndex("dbo.SpeakerTags", new[] { "Speaker_Id" });
            DropColumn("dbo.Tags", "Type");
            DropTable("dbo.SpeakerTags");
        }
    }
}
