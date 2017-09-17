namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using Probe.Helpers.Mics;
    
    public partial class DataMigrationv12 : DbMigration
    {
        public override void Up()
        {
            //Data
            Sql(ReadSQLScriptFile.Read("03-Copy-All-GamePlay-to-Game-Table.sql"));
            Sql(ReadSQLScriptFile.Read("06-Update-GameId-From-Player-Table-Based-On-Copied-GamePlay-Records.sql"));
            Sql(ReadSQLScriptFile.Read("08-Update-GameId-From-GameQuestion-Table-Based-On-Copied-GamePlay-Records.sql"));
            Sql(ReadSQLScriptFile.Read("081-Update-GameId-From-GameConfiguration-Table-Based-On-Copied-GamePlay-Records.sql"));
            Sql(ReadSQLScriptFile.Read("09-Delete-OLD-Games.sql"));
            Sql(ReadSQLScriptFile.Read("10-Delete-Corrupt-Player-Without-GameAnswer-Records.sql"));
            Sql(ReadSQLScriptFile.Read("20-InsertLMSGameType.sql"));

            //Programmability
            //Sql(ReadSQLScriptFile.Read(@"programmability\15-Drop-GetGamePlayforCode-SP.sql"));
            //Sql(ReadSQLScriptFile.Read(@"programmability\20-Drop-FncPlayerMatchMinMax-Fnc.sql"));
            //Sql(ReadSQLScriptFile.Read(@"programmability\FncPercentChosenPerChoice-ALTER-UDF.sql"));
            //Sql(ReadSQLScriptFile.Read(@"programmability\FncSelectedChoicesForQuestion-ALTER-UDF.sql"));
            //Sql(ReadSQLScriptFile.Read(@"programmability\GetGameforCode-CREATESP.sql"));
            //Sql(ReadSQLScriptFile.Read(@"programmability\GetGamePlayerMatchMinMax-ALTERSP.sql"));
            //Sql(ReadSQLScriptFile.Read(@"programmability\GetPlayerMatchDetail-ALTERSP.sql"));
            //Sql(ReadSQLScriptFile.Read(@"programmability\GetPlayerMatchSummary-CreateSP.sql"));
            //Sql(ReadSQLScriptFile.Read(@"programmability\GetPlayerTestDetail-ALTERSP.sql"));
            //Sql(ReadSQLScriptFile.Read(@"programmability\GetPlayerTestSummary-ALTERSP.sql"));

        }
        
        public override void Down()
        {
        }
    }
}
