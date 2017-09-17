namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Probe.Helpers.Mics;
    
    public partial class SeedConfigurationAndRemoveGameConfigurationTablev12 : DbMigration
    {
        public override void Up()
        {
            Sql(ReadSQLScriptFile.Read("15-InsertGlobalConfigurationRecordsAndRemoveAllGameConfigurationRecords.sql"));

            string aboutHTML = ReadSQLScriptFile.Read("AboutContent.html");
            aboutHTML.Replace("'", "''");

            string sqlStatement = "INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value) VALUES ('InCommon-About','InCommon client about page',0,0,'"
                + aboutHTML.Replace("'", "''") + "')";
            //read in the about html
            Sql(sqlStatement);

            //We can add the foreign key back into GameConfiguration because we have removed all its records.
            AddForeignKey("dbo.GameConfiguration", "ConfigurationGId", "dbo.ConfigurationG", "Id", cascadeDelete: true);

        }
        
        public override void Down()
        {
        }
    }
}
