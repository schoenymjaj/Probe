namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Probe.Helpers.Mics;
    
    public partial class SeedGroupTableAndConfigurtationGRefToGroupTable : DbMigration
    {
        public override void Up()
        {
            Sql(ReadSQLScriptFile.Read("v1.4-InsertGroup&SetGroupInConfigurationTable.sql"));
        }
        
        public override void Down()
        {
        }
    }
}
