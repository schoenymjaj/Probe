namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {

/*
            CreateTable(
                "dbo.Choice",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ChoiceQuestionId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 60),
                        Text = c.String(maxLength: 300),
                        OrderNbr = c.Long(nullable: false),
                        Correct = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChoiceQuestion", t => t.ChoiceQuestionId)
                .Index(t => t.ChoiceQuestionId);
            
            CreateTable(
                "dbo.Question",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AspNetUsersId = c.String(nullable: false, maxLength: 128),
                        QuestionTypeId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 60),
                        Text = c.String(maxLength: 300),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuestionType", t => t.QuestionTypeId, cascadeDelete: true)
                .Index(t => t.QuestionTypeId);
            
            CreateTable(
                "dbo.GameQuestion",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GameId = c.Long(nullable: false),
                        QuestionId = c.Long(nullable: false),
                        OrderNbr = c.Long(nullable: false),
                        Weight = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Game", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.Question", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.GameId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Game",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AspNetUsersId = c.String(nullable: false, maxLength: 128),
                        GameTypeId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 60),
                        Description = c.String(maxLength: 200),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GameType", t => t.GameTypeId, cascadeDelete: true)
                .Index(t => t.GameTypeId);
            
            CreateTable(
                "dbo.GameConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        GameId = c.Long(nullable: false),
                        Name = c.String(nullable: false, maxLength: 60),
                        Description = c.String(maxLength: 200),
                        Value = c.String(nullable: false, maxLength: 200),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Game", t => t.GameId, cascadeDelete: true)
                .Index(t => t.GameId);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Game", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.ReportType", t => t.ReportType_Id)
                .Index(t => t.GameId)
                .Index(t => t.ReportType_Id);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GamePlay", t => t.GamePlayId, cascadeDelete: true)
                .ForeignKey("dbo.ReportType", t => t.ReportTypeId, cascadeDelete: true)
                .Index(t => t.GamePlayId)
                .Index(t => t.ReportTypeId);
            
            CreateTable(
                "dbo.ReportType",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        Description = c.String(maxLength: 200),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Person",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LastName = c.String(maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        MiddleName = c.String(maxLength: 1),
                        NickName = c.String(nullable: false, maxLength: 50),
                        EmailAddr = c.String(),
                        MobileNbr = c.String(),
                        Sex = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GamePlayAnswer",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PlayerId = c.Long(nullable: false),
                        ChoiceId = c.Long(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Choice", t => t.ChoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Player", t => t.PlayerId)
                .Index(t => t.PlayerId)
                .Index(t => t.ChoiceId);
            
            CreateTable(
                "dbo.GameType",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        Description = c.String(maxLength: 200),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuestionType",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        Description = c.String(maxLength: 200),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChoiceQuestion",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        OneChoice = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Question", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Player",
                c => new
                    {
                        Id = c.Long(nullable: false),
                        GamePlayId = c.Long(nullable: false),
                        SubmitDate = c.DateTime(nullable: false),
                        SubmitTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Person", t => t.Id)
                .ForeignKey("dbo.GamePlay", t => t.GamePlayId, cascadeDelete: true)
                .Index(t => t.Id)
                .Index(t => t.GamePlayId);
*/            
        }
        
        public override void Down()
        {
/*
            DropForeignKey("dbo.Player", "GamePlayId", "dbo.GamePlay");
            DropForeignKey("dbo.Player", "Id", "dbo.Person");
            DropForeignKey("dbo.ChoiceQuestion", "Id", "dbo.Question");
            DropForeignKey("dbo.Choice", "ChoiceQuestionId", "dbo.ChoiceQuestion");
            DropForeignKey("dbo.GameQuestion", "QuestionId", "dbo.Question");
            DropForeignKey("dbo.Question", "QuestionTypeId", "dbo.QuestionType");
            DropForeignKey("dbo.GameQuestion", "GameId", "dbo.Game");
            DropForeignKey("dbo.Game", "GameTypeId", "dbo.GameType");
            DropForeignKey("dbo.GamePlayAnswer", "PlayerId", "dbo.Player");
            DropForeignKey("dbo.GamePlayAnswer", "ChoiceId", "dbo.Choice");
            DropForeignKey("dbo.GamePlayReport", "ReportTypeId", "dbo.ReportType");
            DropForeignKey("dbo.GamePlay", "ReportType_Id", "dbo.ReportType");
            DropForeignKey("dbo.GamePlayReport", "GamePlayId", "dbo.GamePlay");
            DropForeignKey("dbo.GamePlay", "GameId", "dbo.Game");
            DropForeignKey("dbo.GameConfiguration", "GameId", "dbo.Game");
            DropIndex("dbo.Player", new[] { "GamePlayId" });
            DropIndex("dbo.Player", new[] { "Id" });
            DropIndex("dbo.ChoiceQuestion", new[] { "Id" });
            DropIndex("dbo.GamePlayAnswer", new[] { "ChoiceId" });
            DropIndex("dbo.GamePlayAnswer", new[] { "PlayerId" });
            DropIndex("dbo.GamePlayReport", new[] { "ReportTypeId" });
            DropIndex("dbo.GamePlayReport", new[] { "GamePlayId" });
            DropIndex("dbo.GamePlay", new[] { "ReportType_Id" });
            DropIndex("dbo.GamePlay", new[] { "GameId" });
            DropIndex("dbo.GameConfiguration", new[] { "GameId" });
            DropIndex("dbo.Game", new[] { "GameTypeId" });
            DropIndex("dbo.GameQuestion", new[] { "QuestionId" });
            DropIndex("dbo.GameQuestion", new[] { "GameId" });
            DropIndex("dbo.Question", new[] { "QuestionTypeId" });
            DropIndex("dbo.Choice", new[] { "ChoiceQuestionId" });
            DropTable("dbo.Player");
            DropTable("dbo.ChoiceQuestion");
            DropTable("dbo.QuestionType");
            DropTable("dbo.GameType");
            DropTable("dbo.GamePlayAnswer");
            DropTable("dbo.Person");
            DropTable("dbo.ReportType");
            DropTable("dbo.GamePlayReport");
            DropTable("dbo.GamePlay");
            DropTable("dbo.GameConfiguration");
            DropTable("dbo.Game");
            DropTable("dbo.GameQuestion");
            DropTable("dbo.Question");
            DropTable("dbo.Choice");
*/
        }
    }
}
