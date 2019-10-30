namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSessionizeSpeakerId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Speakers", "SessionizeId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Speakers", "SessionizeId");
        }
    }
}
