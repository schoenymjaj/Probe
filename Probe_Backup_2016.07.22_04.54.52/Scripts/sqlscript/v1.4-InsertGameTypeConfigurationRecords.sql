/*
Insert GameTypeId - ConfigurationG joins to specify appropriate configuration parms for a game type 
*/
--DeviceCanPlayGameOnlyOnce
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,1)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,1)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,1)

--MinNameLengthPrompted
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,3)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,3)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,3)

--MaxNameLengthPrompted
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,4)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,4)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,4)

--PlayerFirstNamePrompted
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,5)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,5)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,5)

--PlayerNickNamePrompted
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,6)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,6)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,6)

--PlayerSexPrompted
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,7)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,7)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,7)

--PlayerLastNamePrompted
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,8)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,8)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,8)

--PlayerEmailPrompted
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,9)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,9)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,9)

--PlayerCellPhonePrompted
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,10)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,10)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,10)

--FindGameTimeCompleteInSecs
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,11)

--FirstQuestionTimeCompleteInSecs
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,12)

--QuestionTimeCompleteInSecs
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,13)

--QuestionTimeWarningInSecs
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,14)

--QuestionTimeSlopeInSecs
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,15)

--ViewNbrOfQuestionsTotal
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,16)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,16)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,16)

--AnswerSubmitSuccessMessage
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,17)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,17)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,17)

--AnswerSubmitFailMessage
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,18)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,18)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,18)

--SubmitConfirmMessage
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (1,19)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (2,19)
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,19)

--QuestionTimeWarningMessage
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,20)

--QuestionTimeDeadlineMessage
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,21)

--ServerClientTimeSyncInSecs
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,23)

--CountdownIntervalInSecs
INSERT INTO GameTypeConfiguration (GameTypeId, ConfigurationGId)
VALUES (3,24)








