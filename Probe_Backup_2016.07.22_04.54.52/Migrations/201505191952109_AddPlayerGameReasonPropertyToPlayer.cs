namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlayerGameReasonPropertyToPlayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Player", "PlayerGameReason", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Player", "PlayerGameReason");
        }
    }
}
