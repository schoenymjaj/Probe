namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActivePropertyToPlayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Player", "Active", c => c.Boolean(nullable: false));

            //Set all existing players to active
            Sql("UPDATE player SET Active = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Player", "Active");
        }
    }
}
