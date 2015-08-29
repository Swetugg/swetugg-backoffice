namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TravelAndAccommodation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CfpSpeakers", "NeedTravel", c => c.Boolean(nullable: false));
            AddColumn("dbo.CfpSpeakers", "NeedAccommodation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CfpSpeakers", "NeedAccommodation");
            DropColumn("dbo.CfpSpeakers", "NeedTravel");
        }
    }
}
