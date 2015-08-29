namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupFKToConfigurtationGTable : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.ConfigurationG", "GroupId", "dbo.Group", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
        }
    }
}
