namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPublishedPropertyToGame : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Game", "Published", c => c.Boolean(nullable: false));

            //Update each game TODATE to published state
            Sql("UPDATE Game SET Published = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Game", "Published");
        }
    }
}
