/*
Copy all GamePlay records to Game table. Set CreatedBy to 'v1.2 MIGRATION'
The name and description of new Game records will be set by the name and description of GamePlay records
*/
DECLARE @dateTimeNow DATETIME
SET @dateTimeNow =  GETDATE()

INSERT INTO Game (AspNetUsersId,GameTypeId,Name,Description
,DateCreated,CreatedBy,DateUpdated,UpdatedBy
,ACLId,Code,GameUrl,StartDate,EndDate,SuspendMode
,ClientReportAccess,TestMode,ReportType_Id) 
SELECT g.AspNetUsersId,g.GameTypeId,gp.Name,gp.Description
,@dateTimeNow AS DateCreated,'v1.2 MIGRATION' AS CreatedBy,NULL AS DateUpdated,NULL AS UpdatedBy
,g.ACLId,gp.Code,gp.GameUrl,gp.StartDate,gp.EndDate,gp.SuspendMode
,gp.ClientReportAccess,gp.TestMode,gp.ReportType_Id
FROM GamePlay gp
INNER JOIN Game g on g.id = gp.GameId
