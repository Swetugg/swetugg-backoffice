namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SponsorImages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SponsorImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SponsorId = c.Int(nullable: false),
                        ImageTypeId = c.Int(nullable: false),
                        ImageUrl = c.String(nullable: false, maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ImageTypes", t => t.ImageTypeId)
                .ForeignKey("dbo.Sponsors", t => t.SponsorId)
                .Index(t => t.SponsorId)
                .Index(t => t.ImageTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SponsorImages", "SponsorId", "dbo.Sponsors");
            DropForeignKey("dbo.SponsorImages", "ImageTypeId", "dbo.ImageTypes");
            DropIndex("dbo.SponsorImages", new[] { "ImageTypeId" });
            DropIndex("dbo.SponsorImages", new[] { "SponsorId" });
            DropTable("dbo.SponsorImages");
        }
    }
}
