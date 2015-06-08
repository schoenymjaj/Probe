namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGameIdFKforPlayerAndGameQuestionv12 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Player", "GamePlayId");
            AddForeignKey("dbo.Player", "GameId", "dbo.Game", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GameQuestion", "GameId", "dbo.Game", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
        }
    }
}
