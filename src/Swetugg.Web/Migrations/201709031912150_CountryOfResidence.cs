namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CountryOfResidence : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CfpSpeakers", "CountryOfResidence", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CfpSpeakers", "CountryOfResidence");
        }
    }
}
