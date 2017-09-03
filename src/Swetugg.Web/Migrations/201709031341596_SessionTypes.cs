namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SessionTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SessionTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConferenceId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        Slug = c.String(nullable: false, maxLength: 250),
                        Description = c.String(storeType: "ntext"),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Conferences", t => t.ConferenceId)
                .Index(t => t.ConferenceId);
            
            AddColumn("dbo.CfpSessions", "SessionTypeId", c => c.Int());
            AddColumn("dbo.Sessions", "SessionTypeId", c => c.Int());
            CreateIndex("dbo.CfpSessions", "SessionTypeId");
            CreateIndex("dbo.Sessions", "SessionTypeId");
            AddForeignKey("dbo.Sessions", "SessionTypeId", "dbo.SessionTypes", "Id");
            AddForeignKey("dbo.CfpSessions", "SessionTypeId", "dbo.SessionTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SessionTypes", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.CfpSessions", "SessionTypeId", "dbo.SessionTypes");
            DropForeignKey("dbo.Sessions", "SessionTypeId", "dbo.SessionTypes");
            DropIndex("dbo.SessionTypes", new[] { "ConferenceId" });
            DropIndex("dbo.Sessions", new[] { "SessionTypeId" });
            DropIndex("dbo.CfpSessions", new[] { "SessionTypeId" });
            DropColumn("dbo.Sessions", "SessionTypeId");
            DropColumn("dbo.CfpSessions", "SessionTypeId");
            DropTable("dbo.SessionTypes");
        }
    }
}
