namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSession : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "SessionizeId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "SessionizeId");
        }
    }
}
