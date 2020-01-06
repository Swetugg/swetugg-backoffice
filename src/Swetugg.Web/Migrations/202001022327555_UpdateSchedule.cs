namespace Swetugg.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateSchedule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoomSlots", "Start", c => c.DateTime());
            AddColumn("dbo.RoomSlots", "End", c => c.DateTime());
            AddColumn("dbo.Slots", "HasSessions", c => c.Boolean(nullable: false, defaultValue: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Slots", "HasSessions");
            DropColumn("dbo.RoomSlots", "End");
            DropColumn("dbo.RoomSlots", "Start");
        }
    }
}
