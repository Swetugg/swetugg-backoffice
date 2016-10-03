namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastUpdateOnCfp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CfpSessions", "LastUpdate", c => c.DateTime());
            AddColumn("dbo.CfpSpeakers", "LastUpdate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CfpSpeakers", "LastUpdate");
            DropColumn("dbo.CfpSessions", "LastUpdate");
        }
    }
}
