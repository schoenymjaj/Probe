namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShortDescToGroupTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfigurationG", "ShortDescription", c => c.String(maxLength: 60));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConfigurationG", "ShortDescription");
        }
    }
}
