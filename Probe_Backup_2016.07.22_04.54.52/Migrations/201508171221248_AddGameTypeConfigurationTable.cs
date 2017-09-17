namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Probe.Helpers.Mics;
    
    public partial class AddGameTypeConfigurationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameTypeConfiguration",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ConfigurationGId = c.Long(nullable: false),
                        GameTypeId = c.Long(nullable: false),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ConfigurationG", t => t.ConfigurationGId, cascadeDelete: true)
                .ForeignKey("dbo.GameType", t => t.GameTypeId, cascadeDelete: true)
                .Index(t => t.ConfigurationGId)
                .Index(t => t.GameTypeId);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameTypeConfiguration", "GameTypeId", "dbo.GameType");
            DropForeignKey("dbo.GameTypeConfiguration", "ConfigurationGId", "dbo.ConfigurationG");
            DropIndex("dbo.GameTypeConfiguration", new[] { "GameTypeId" });
            DropIndex("dbo.GameTypeConfiguration", new[] { "ConfigurationGId" });
            DropTable("dbo.GameTypeConfiguration");
        }
    }
}
