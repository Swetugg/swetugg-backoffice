namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OpenCloseCfp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Conferences", "CfpStart", c => c.DateTime());
            AddColumn("dbo.Conferences", "CfpEnd", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Conferences", "CfpEnd");
            DropColumn("dbo.Conferences", "CfpStart");
        }
    }
}
