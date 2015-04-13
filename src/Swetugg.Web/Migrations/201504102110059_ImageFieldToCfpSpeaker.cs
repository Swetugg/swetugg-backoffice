namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageFieldToCfpSpeaker : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CfpSpeakers", "Image", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CfpSpeakers", "Image");
        }
    }
}
