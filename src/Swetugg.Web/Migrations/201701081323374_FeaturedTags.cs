namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FeaturedTags : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tags", "Featured", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tags", "Featured");
        }
    }
}
