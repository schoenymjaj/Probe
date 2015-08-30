	UPDATE GameConfiguration 
	SET
		GameConfiguration.GameId = fromTbl.NEWgId
	FROM
	(
		SELECT gpMatch.NEWgId, gpMatch.OLDgId AS OLDgId
		FROM GameConfiguration gc
		INNER JOIN
		(
			SELECT gp.Id AS OLDgpId, g.Id AS NEWgId, gp.GameId AS OLDgId
			FROM GamePlay gp
			INNER JOIN Game g ON g.Code = gp.Code
			WHERE g.CreatedBy = 'v1.2 MIGRATION'
		) gpMatch ON gpMatch.OLDgId = gc.GameId
	) fromTbl
	WHERE GameConfiguration.GameId = fromTbl.OLDgId


