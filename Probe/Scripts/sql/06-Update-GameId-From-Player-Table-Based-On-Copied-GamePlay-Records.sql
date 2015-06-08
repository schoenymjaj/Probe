/*
Update Player - GameId field to the new value based on the new records in the Game table that match the 
GamePlay table records copied into the Game table (CreatedBy - 'v1.2 MIGRATION', name, description, code)
*/

UPDATE Player 
SET
	Player.GameId = fromTbl.NEWgId
FROM
(
	SELECT gpMatch.NEWgId, gpMatch.OLDgpId AS OLDgpId
	FROM Player p
	INNER JOIN
	(
		SELECT gp.Id AS OLDgpId, g.Id AS NEWgId
		FROM GamePlay gp
		INNER JOIN Game g ON g.Code = gp.Code
		WHERE g.CreatedBy = 'v1.2 MIGRATION'
	) gpMatch ON gpMatch.OLDgpId = p.GamePlayId
) fromTbl
WHERE Player.GamePlayId = fromTbl.OLDgpId

