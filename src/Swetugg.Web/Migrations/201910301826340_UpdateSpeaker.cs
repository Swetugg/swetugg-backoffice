namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSpeaker : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Speakers", "FirstName", c => c.String(nullable: false, maxLength: 250));
            AddColumn("dbo.Speakers", "SessionizeImageUrl", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Speakers", "SessionizeImageUrl");
            DropColumn("dbo.Speakers", "FirstName");
        }
    }
}
