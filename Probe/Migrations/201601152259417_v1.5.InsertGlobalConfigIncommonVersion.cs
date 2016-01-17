namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Probe.Helpers.Mics;
    
    public partial class v15InsertGlobalConfigIncommonVersion : DbMigration
    {
        public override void Up()
        {
            Sql(ReadSQLScriptFile.Read("v.1.5-InsertGlobalConfiguration-InCommon-Version.sql"));

        }
        
        public override void Down()
        {
        }
    }
}
