namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SessionTags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConferenceId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        Slug = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SessionTags",
                c => new
                    {
                        Session_Id = c.Int(nullable: false),
                        Tag_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Session_Id, t.Tag_Id })
                .ForeignKey("dbo.Sessions", t => t.Session_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .Index(t => t.Session_Id)
                .Index(t => t.Tag_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SessionTags", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.SessionTags", "Session_Id", "dbo.Sessions");
            DropIndex("dbo.SessionTags", new[] { "Tag_Id" });
            DropIndex("dbo.SessionTags", new[] { "Session_Id" });
            DropTable("dbo.SessionTags");
            DropTable("dbo.Tags");
        }
    }
}
