namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Group",
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
            
            AddColumn("dbo.ConfigurationG", "GroupId", c => c.Long(nullable: false));
            CreateIndex("dbo.ConfigurationG", "GroupId");
            
            //Not yet - we will have to seed ConfigurationG first
            //AddForeignKey("dbo.ConfigurationG", "GroupId", "dbo.Group", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConfigurationG", "GroupId", "dbo.Group");
            DropIndex("dbo.ConfigurationG", new[] { "GroupId" });
            DropColumn("dbo.ConfigurationG", "GroupId");
            DropTable("dbo.Group");
        }
    }
}
