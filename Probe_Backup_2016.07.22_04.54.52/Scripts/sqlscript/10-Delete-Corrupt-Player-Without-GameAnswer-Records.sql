/*
Delete corrupt Players that do not have GameAnswer records
*/
DECLARE @PlayerWithNoGameQuestion TABLE
(
	Id BIGINT NOT NULL
)

INSERT INTO @PlayerWithNoGameQuestion (Id)
SELECT DISTINCT p.id
FROM GameAnswer ga
RIGHT JOIN Player p on p.Id = ga.PlayerId
WHERE p.GameId = 18 AND ga.id IS NULL

DELETE Player
WHERE Id IN 
(SELECT Id FROM @PlayerWithNoGameQuestion)

DELETE Person
WHERE Id IN 
(SELECT Id FROM @PlayerWithNoGameQuestion)

