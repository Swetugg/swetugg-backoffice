namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SwetuggDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CfpSessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConferenceId = c.Int(nullable: false),
                        SpeakerId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        Tags = c.String(nullable: false, maxLength: 250),
                        Audience = c.String(nullable: false, maxLength: 250),
                        Level = c.String(nullable: false, maxLength: 250),
                        Description = c.String(storeType: "ntext"),
                        Comments = c.String(storeType: "ntext"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CfpSpeakers", t => t.SpeakerId)
                .Index(t => t.SpeakerId);
            
            CreateTable(
                "dbo.CfpSpeakers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConferenceId = c.Int(nullable: false),
                        UserId = c.String(),
                        Name = c.String(nullable: false, maxLength: 250),
                        Company = c.String(maxLength: 250),
                        Bio = c.String(storeType: "ntext"),
                        Email = c.String(maxLength: 250),
                        Web = c.String(maxLength: 250),
                        Twitter = c.String(maxLength: 50),
                        GitHub = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Conferences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        Slug = c.String(nullable: false, maxLength: 250),
                        Description = c.String(storeType: "ntext"),
                        Start = c.DateTime(),
                        End = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rooms",
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
            
            CreateTable(
                "dbo.RoomSlots",
                c => new
                    {
                        RoomId = c.Int(nullable: false),
                        SlotId = c.Int(nullable: false),
                        AssignedSessionId = c.Int(),
                        IsChanged = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoomId, t.SlotId })
                .ForeignKey("dbo.Sessions", t => t.AssignedSessionId)
                .ForeignKey("dbo.Rooms", t => t.RoomId)
                .ForeignKey("dbo.Slots", t => t.SlotId)
                .Index(t => t.RoomId)
                .Index(t => t.SlotId)
                .Index(t => t.AssignedSessionId);
            
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConferenceId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        Slug = c.String(nullable: false, maxLength: 250),
                        Description = c.String(storeType: "ntext"),
                        VideoUrl = c.String(maxLength: 500),
                        VideoPublished = c.Boolean(nullable: false),
                        Published = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Conferences", t => t.ConferenceId)
                .Index(t => t.ConferenceId);
            
            CreateTable(
                "dbo.SessionSpeakers",
                c => new
                    {
                        SessionId = c.Int(nullable: false),
                        SpeakerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SessionId, t.SpeakerId })
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .ForeignKey("dbo.Speakers", t => t.SpeakerId)
                .Index(t => t.SessionId)
                .Index(t => t.SpeakerId);
            
            CreateTable(
                "dbo.Speakers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConferenceId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        Company = c.String(maxLength: 250),
                        Slug = c.String(nullable: false, maxLength: 250),
                        Bio = c.String(storeType: "ntext"),
                        Web = c.String(maxLength: 250),
                        Twitter = c.String(maxLength: 50),
                        GitHub = c.String(maxLength: 250),
                        Published = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Conferences", t => t.ConferenceId)
                .Index(t => t.ConferenceId);
            
            CreateTable(
                "dbo.Slots",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConferenceId = c.Int(nullable: false),
                        Title = c.String(maxLength: 250),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Conferences", t => t.ConferenceId)
                .Index(t => t.ConferenceId);
            
            CreateTable(
                "dbo.Sponsors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ConferenceId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        Slug = c.String(nullable: false, maxLength: 250),
                        Description = c.String(storeType: "ntext"),
                        Web = c.String(maxLength: 250),
                        Twitter = c.String(maxLength: 50),
                        Published = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Conferences", t => t.ConferenceId)
                .Index(t => t.ConferenceId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Sponsors", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.Speakers", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.Slots", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.Sessions", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.Rooms", "ConferenceId", "dbo.Conferences");
            DropForeignKey("dbo.RoomSlots", "SlotId", "dbo.Slots");
            DropForeignKey("dbo.RoomSlots", "RoomId", "dbo.Rooms");
            DropForeignKey("dbo.RoomSlots", "AssignedSessionId", "dbo.Sessions");
            DropForeignKey("dbo.SessionSpeakers", "SpeakerId", "dbo.Speakers");
            DropForeignKey("dbo.SessionSpeakers", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.CfpSessions", "SpeakerId", "dbo.CfpSpeakers");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Sponsors", new[] { "ConferenceId" });
            DropIndex("dbo.Slots", new[] { "ConferenceId" });
            DropIndex("dbo.Speakers", new[] { "ConferenceId" });
            DropIndex("dbo.SessionSpeakers", new[] { "SpeakerId" });
            DropIndex("dbo.SessionSpeakers", new[] { "SessionId" });
            DropIndex("dbo.Sessions", new[] { "ConferenceId" });
            DropIndex("dbo.RoomSlots", new[] { "AssignedSessionId" });
            DropIndex("dbo.RoomSlots", new[] { "SlotId" });
            DropIndex("dbo.RoomSlots", new[] { "RoomId" });
            DropIndex("dbo.Rooms", new[] { "ConferenceId" });
            DropIndex("dbo.CfpSessions", new[] { "SpeakerId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Sponsors");
            DropTable("dbo.Slots");
            DropTable("dbo.Speakers");
            DropTable("dbo.SessionSpeakers");
            DropTable("dbo.Sessions");
            DropTable("dbo.RoomSlots");
            DropTable("dbo.Rooms");
            DropTable("dbo.Conferences");
            DropTable("dbo.CfpSpeakers");
            DropTable("dbo.CfpSessions");
        }
    }
}
