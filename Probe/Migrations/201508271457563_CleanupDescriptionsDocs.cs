namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Probe.Helpers.Mics;
    
    public partial class CleanupDescriptionsDocs : DbMigration
    {
        public override void Up()
        {
            Sql(ReadSQLScriptFile.Read("v1.4-CleanupDescriptionsDocs.sql"));
        }
        
        public override void Down()
        {
        }
    }
}
