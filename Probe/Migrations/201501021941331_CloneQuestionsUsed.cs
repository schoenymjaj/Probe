namespace Probe.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CloneQuestionsUsed : DbMigration
    {
        public override void Up()
        {
            string SqlCreateIndex;

            //Create UsedInGame Index
            SqlCreateIndex = @"CREATE NONCLUSTERED INDEX [NonClusteredIndex-20150102-150423-UsedInGame] ON [dbo].[Question]
                                (
	                                [UsedInGame] ASC
                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)";
            Sql(SqlCreateIndex);

            //Create UsedInGame, AspNetUsersId Index
            SqlCreateIndex = @"CREATE NONCLUSTERED INDEX [NonClusteredIndex-20150102-150728-UsedInGame-AspNetUsersId] ON [dbo].[Question]
                            (
	                            [AspNetUsersId] ASC,
	                            [UsedInGame] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)";
            Sql(SqlCreateIndex);

            //Drop existing NonClusteredIndex-Question-Unique-AspNetUserId-Name because we are going to reinvent it next
            Sql("DROP INDEX [NonClusteredIndex-Question-Unique-AspNetUserId-Name] ON [dbo].[Question]");


            //Create UsedInGame,  AspNetUsersId, Name UNIQUE index
            SqlCreateIndex = @"CREATE NONCLUSTERED INDEX [NonClusteredIndex-Question-AspNetUserId-Name-UsedInGame] ON [dbo].[Question]
                                (
	                                [AspNetUsersId] ASC,
	                                [Name] ASC,
	                                [UsedInGame] ASC
                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)";
            Sql(SqlCreateIndex);

            //Update the UsedInGame field if the question is being used.
            Sql("UPDATE Question SET UsedInGame = 1 WHERE Id IN (SELECT DISTINCT QuestionId FROM GameQuestion)");

            //Update the questions that aren't used
            Sql("UPDATE Question SET UsedInGame = 0 WHERE UsedInGame is Null");

            //copy questions
            Sql(@"INSERT INTO Question (AspNetUsersId,QuestionTypeId,Name,[Text],DateCreated,CreatedBy,DateUpdated,UpdatedBy,UsedInGame,ACLId)
                    SELECT
                    AspNetUsersId
                    ,QuestionTypeId
                    ,Name
                    ,[Text]
                    ,GetDate() AS DateCreated
                    ,'DataMigration' AS CreatedBy
                    ,DateUpdated
                    ,UpdatedBy
                    ,0 AS UsedInGame
                    ,ACLId
                    FROM Question
                    WHERE UsedInGame = 1");

            //copy choicequestions
            Sql(@"INSERT INTO ChoiceQuestion(Id, OneChoice)
                    SELECT
                    q.Id
                    ,1 AS OneChoice 
                    FROM Question q 
                    LEFT JOIN ChoiceQuestion cq on cq.Id = q.Id
                    WHERE cq.Id is null");

            //copy choices
            Sql(@"INSERT INTO Choice (ChoiceQuestionId, [Name], [Text], OrderNbr, Correct, DateCreated, CreatedBy)
                    SELECT qMigrated.Id AS ChoiceQuestionId
                    ,c.Name AS Name, c.[Text] AS [Text], c.OrderNbr AS OrderNbr, c.Correct AS Correct, GETDATE(), 'DataMigration'
                    FROM Question qMigrated
                    INNER JOIN Question qBefore ON qBefore.Name = qMigrated.Name
                    INNER JOIN Choice c ON c.ChoiceQuestionId = qBefore.Id
                    WHERE (qMigrated.UsedInGame = 0 AND qMigrated.CreatedBy = 'DataMigration') AND (qBefore.UsedInGame = 1)");

        }
        
        public override void Down()
        {
        }
    }
}
