namespace DeviceMonitor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitailCreate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceData", "AlarmAble", c => c.Boolean(nullable: false));
            AddColumn("dbo.DeviceData", "upper", c => c.Int());
            AddColumn("dbo.DeviceData", "lower", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeviceData", "lower");
            DropColumn("dbo.DeviceData", "upper");
            DropColumn("dbo.DeviceData", "AlarmAble");
        }
    }
}
