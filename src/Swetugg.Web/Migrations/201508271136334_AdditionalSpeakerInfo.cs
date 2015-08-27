namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdditionalSpeakerInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CfpSpeakers", "Comments", c => c.String(storeType: "ntext"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CfpSpeakers", "Comments");
        }
    }
}
