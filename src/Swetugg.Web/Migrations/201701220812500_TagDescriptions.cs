namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TagDescriptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tags", "Description", c => c.String(storeType: "ntext"));
            AddColumn("dbo.Tags", "Priority", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tags", "Priority");
            DropColumn("dbo.Tags", "Description");
        }
    }
}
