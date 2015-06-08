/*
Update GameQuestion - GameId field to the new value based on the new records in the Game table that match the 
GamePlay table records copied into the Game table (CreatedBy - 'v1.2 MIGRATION', name, description, code)

THERE EXISTS GAME PLAYS THAT HAVE THE SAME GAME ID. HOW TO SUPPORT THIS?
FOR EACH GamePlay. THERE MUST BE A UNIQUE SET OF GameQuestion's and UNIQUE SET
OF Question's AND Choice's
*/


IF EXISTS
(
SELECT GameId,COUNT(*) FROM GamePlay
GROUP BY GameId
HAVING COUNT(*) > 1
) 
BEGIN
	PRINT 'THERE ARE MULTIPLE GAMEPLAY RECORDS FOR A GAME. PLEASE REMOVE THOSE RECORDS BEFORE THIS SCRIPT CAN BE SUCCESSFUL'
END
ELSE
BEGIN
	UPDATE GameQuestion 
	SET
		GameQuestion.GameId = fromTbl.NEWgId
	FROM
	(
		SELECT gpMatch.NEWgId, gpMatch.OLDgId AS OLDgId
		FROM GameQuestion gq
		INNER JOIN
		(
			SELECT gp.Id AS OLDgpId, g.Id AS NEWgId, gp.GameId AS OLDgId
			FROM GamePlay gp
			INNER JOIN Game g ON g.Code = gp.Code
			WHERE g.CreatedBy = 'v1.2 MIGRATION'
		) gpMatch ON gpMatch.OLDgId = gq.GameId
	) fromTbl
	WHERE GameQuestion.GameId = fromTbl.OLDgId
END


