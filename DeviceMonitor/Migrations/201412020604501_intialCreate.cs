namespace DeviceMonitor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class intialCreate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeviceDataFormat", "index", c => c.Int(nullable: false));
            AddColumn("dbo.DeviceInfo", "index", c => c.Int(nullable: false));
            AddColumn("dbo.DeviceGroup", "index", c => c.Int(nullable: false));
            AddColumn("dbo.DeviceFactory", "index", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeviceFactory", "index");
            DropColumn("dbo.DeviceGroup", "index");
            DropColumn("dbo.DeviceInfo", "index");
            DropColumn("dbo.DeviceDataFormat", "index");
        }
    }
}
