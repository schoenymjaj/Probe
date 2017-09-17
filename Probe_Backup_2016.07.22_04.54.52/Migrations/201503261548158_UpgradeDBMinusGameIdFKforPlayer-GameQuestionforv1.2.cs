namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpgradeDBMinusGameIdFKforPlayerGameQuestionforv12 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.GamePlayAnswer", newName: "GameAnswer");
            DropForeignKey("dbo.GamePlay", "GameId", "dbo.Game");
            DropForeignKey("dbo.GamePlayReport", "GamePlayId", "dbo.GamePlay");
            DropForeignKey("dbo.GamePlay", "ReportType_Id", "dbo.ReportType");
            DropForeignKey("dbo.GamePlayReport", "ReportTypeId", "dbo.ReportType");
            DropForeignKey("dbo.Player", "GamePlayId", "dbo.GamePlay");
            DropIndex("dbo.GamePlay", new[] { "GameId" });
            DropIndex("dbo.GamePlay", new[] { "ReportType_Id" });
            DropIndex("dbo.GamePlayReport", new[] { "GamePlayId" });
            DropIndex("dbo.GamePlayReport", new[] { "ReportTypeId" });
            DropIndex("dbo.Player", new[] { "GamePlayId" });
            AddColumn("dbo.Game", "Code", c => c.String(maxLength: 60));
            AddColumn("dbo.Game", "GameUrl", c => c.String());
            AddColumn("dbo.Game", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Game", "EndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Game", "SuspendMode", c => c.Boolean(nullable: false));
            AddColumn("dbo.Game", "ClientReportAccess", c => c.Boolean(nullable: false));
            AddColumn("dbo.Game", "TestMode", c => c.Boolean(nullable: false));
            AddColumn("dbo.Game", "ReportType_Id", c => c.Long());
            AddColumn("dbo.Player", "GameId", c => c.Long(nullable: false));
            CreateIndex("dbo.Game", "ReportType_Id");
            CreateIndex("dbo.Player", "GameId");
            AddForeignKey("dbo.Game", "ReportType_Id", "dbo.ReportType", "Id");
            //AddForeignKey("dbo.Player", "GameId", "dbo.Game", "Id", cascadeDelete: true);
            DropForeignKey("dbo.GameQuestion", "GameId", "dbo.Game"); //MNS NEXT MIGRATION STEP
            //DropColumn("dbo.Player", "GamePlayId");
            //DropTable("dbo.GamePlay"); //MNS NEXT MIGRATION STEP
            //DropTable("dbo.GamePlayReport"); //MNS NEXT MIGRATION STEP
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GamePlayReport",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GamePlayId = c.Long(nullable: false),
                        ReportTypeId = c.Long(nullable: false),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GamePlay",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GameId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 60),
                        Description = c.String(maxLength: 200),
                        Code = c.String(nullable: false, maxLength: 60),
                        GameUrl = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        SuspendMode = c.Boolean(nullable: false),
                        ClientReportAccess = c.Boolean(nullable: false),
                        TestMode = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                        ReportType_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Player", "GamePlayId", c => c.Long(nullable: false));
            DropForeignKey("dbo.Player", "GameId", "dbo.Game");
            DropForeignKey("dbo.Game", "ReportType_Id", "dbo.ReportType");
            DropIndex("dbo.Player", new[] { "GameId" });
            DropIndex("dbo.Game", new[] { "ReportType_Id" });
            DropColumn("dbo.Player", "GameId");
            DropColumn("dbo.Game", "ReportType_Id");
            DropColumn("dbo.Game", "TestMode");
            DropColumn("dbo.Game", "ClientReportAccess");
            DropColumn("dbo.Game", "SuspendMode");
            DropColumn("dbo.Game", "EndDate");
            DropColumn("dbo.Game", "StartDate");
            DropColumn("dbo.Game", "GameUrl");
            DropColumn("dbo.Game", "Code");
            CreateIndex("dbo.Player", "GamePlayId");
            CreateIndex("dbo.GamePlayReport", "ReportTypeId");
            CreateIndex("dbo.GamePlayReport", "GamePlayId");
            CreateIndex("dbo.GamePlay", "ReportType_Id");
            CreateIndex("dbo.GamePlay", "GameId");
            AddForeignKey("dbo.Player", "GamePlayId", "dbo.GamePlay", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GamePlayReport", "ReportTypeId", "dbo.ReportType", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GamePlay", "ReportType_Id", "dbo.ReportType", "Id");
            AddForeignKey("dbo.GamePlayReport", "GamePlayId", "dbo.GamePlay", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GamePlay", "GameId", "dbo.Game", "Id", cascadeDelete: true);
            RenameTable(name: "dbo.GameAnswer", newName: "GamePlayAnswer");
        }
    }
}
