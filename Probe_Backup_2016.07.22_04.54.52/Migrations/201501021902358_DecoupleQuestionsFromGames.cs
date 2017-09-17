namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DecoupleQuestionsFromGames : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ACL",
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

            Sql("INSERT INTO dbo.ACL (Name, Description, DateCreated) VALUES('PRIVATE', 'Private Access','2015-01-02')");
            Sql("INSERT INTO dbo.ACL (Name, Description, DateCreated) VALUES('GLOBAL', 'Global Access','2015-01-02')");
            
            AddColumn("dbo.Question", "UsedInGame", c => c.Boolean(nullable: false));
            AddColumn("dbo.Question", "ACLId", c => c.Long(nullable: false));
            AddColumn("dbo.Game", "ACLId", c => c.Long(nullable: false));

            Sql("UPDATE dbo.Question SET UsedInGame = 0");  //false for now (just inialization)
            Sql("UPDATE dbo.Question SET ACLId = (SELECT Id FROM ACL WHERE Name='PRIVATE')");  //false for now (just inialization)
            Sql("UPDATE dbo.Game SET ACLId = (SELECT Id FROM ACL WHERE Name='PRIVATE')");  //false for now (just inialization)

            CreateIndex("dbo.Question", "ACLId");
            CreateIndex("dbo.Game", "ACLId");
            AddForeignKey("dbo.Game", "ACLId", "dbo.ACL", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Question", "ACLId", "dbo.ACL", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Question", "ACLId", "dbo.ACL");
            DropForeignKey("dbo.Game", "ACLId", "dbo.ACL");
            DropIndex("dbo.Game", new[] { "ACLId" });
            DropIndex("dbo.Question", new[] { "ACLId" });
            DropColumn("dbo.Game", "ACLId");
            DropColumn("dbo.Question", "ACLId");
            DropColumn("dbo.Question", "UsedInGame");
            DropTable("dbo.ACL");
        }
    }
}
