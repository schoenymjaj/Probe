namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Probe.Helpers.Mics;
    
    public partial class SeedGameTypeConfigurationTable : DbMigration
    {
        public override void Up()
        {
            Sql(ReadSQLScriptFile.Read("v1.4-InsertGameTypeConfigurationRecords.sql"));
        }
        
        public override void Down()
        {
        }
    }
}
