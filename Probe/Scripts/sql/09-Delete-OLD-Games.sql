/*
Delete all the OLD Game records
*/
DELETE Game 
WHERE Id NOT IN
(
SELECT DISTINCT GameId FROM GameQuestion
) 





