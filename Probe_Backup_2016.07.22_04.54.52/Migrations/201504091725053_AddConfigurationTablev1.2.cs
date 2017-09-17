namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConfigurationTablev12 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConfigurationG",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        Description = c.String(),
                        DataTypeG = c.Int(nullable: false),
                        ConfigurationType = c.Int(nullable: false),
                        Value = c.String(nullable: false, maxLength: 20000),
                        DateCreated = c.DateTime(),
                        CreatedBy = c.String(),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.GameConfiguration", "ConfigurationGId", c => c.Long(nullable: false));
            AddColumn("dbo.GameConfiguration", "DataType", c => c.Int(nullable: false));
            CreateIndex("dbo.GameConfiguration", "ConfigurationGId");
            //AddForeignKey("dbo.GameConfiguration", "ConfigurationGId", "dbo.ConfigurationG", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameConfiguration", "ConfigurationGId", "dbo.ConfigurationG");
            DropIndex("dbo.GameConfiguration", new[] { "ConfigurationGId" });
            DropColumn("dbo.GameConfiguration", "DataType");
            DropColumn("dbo.GameConfiguration", "ConfigurationGId");
            DropTable("dbo.ConfigurationG");
        }
    }
}
