namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CfpVipCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Conferences", "CfpVipCode", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Conferences", "CfpVipCode");
        }
    }
}
