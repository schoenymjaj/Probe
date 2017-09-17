namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateValuePropToConfiguration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GameConfiguration", "Value", c => c.String(nullable: false));
            AlterColumn("dbo.ConfigurationG", "Description", c => c.String(maxLength: 800));
            AlterColumn("dbo.ConfigurationG", "Value", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ConfigurationG", "Value", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.ConfigurationG", "Description", c => c.String());
            AlterColumn("dbo.GameConfiguration", "Value", c => c.String(nullable: false, maxLength: 200));
        }
    }
}
