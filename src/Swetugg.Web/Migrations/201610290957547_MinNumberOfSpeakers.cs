namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinNumberOfSpeakers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Conferences", "MinNumberOfSpeakers", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Conferences", "MinNumberOfSpeakers");
        }
    }
}
