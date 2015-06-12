/*
Insert DeviceCanPlayGameOnlyOnce into Global ConfigurationG table AND Delete All GameConfiguration
We will start will all defaults
*/
INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('DeviceCanPlayGameOnlyOnce','A specific device can only play a game once. If false, this device may play and submit answers multiple times for the same game.',3,1,'false')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('InCommon-CacheMinutes','Time span in minutes that the In Common client will wait before refreshing configuration for a game',1,0,'30')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('MinNameLengthPrompted','Minimum number of characters for a prompted name',1,1,'3')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('MaxNameLengthPrompted','Maxiumum number of characters for a prompted name',1,1,'8')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('PlayerFirstNamePrompted','Player will be promoted for first name',3,1,'true')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('PlayerNickNamePrompted','Player will be promoted for last name',3,1,'true')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('PlayerSexPrompted','Player will be promoted for sex',3,1,'true')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('PlayerLastNamePrompted','Player will be promoted for last name',3,1,'false')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('PlayerEmailPrompted','Player will be promoted for email address',3,1,'false')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('PlayerCellPhonePrompted','Player will be promoted for cell phone number',3,1,'false')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('FindGameTimeCompleteInSecs','Number of seconds player has to complete find game operation',1,1,'300')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('FirstQuestionTimeCompleteInSecs','Number of seconds player has to answer/submit first question of game',1,1,'60')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('QuestionTimeCompleteInSecs','Number of seconds player has to answer/submit all questions after first question of game',1,1,'60')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('QuestionTimeWarningInSecs','Percentage of question time before a player receives a warning if they haven''t answered/submitted a question',1,1,'75')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('QuestionTimeSlopeInSecs','The change in seconds the player has to answer/submit a question. Effective after second question.',1,1,'0')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('ServerClientTimeSyncInSecs','The time range in seconds the server and client(player''s device) clock should be in synchronization before it becomes a problem for the game. Set to value of 0 to make not applicable',1,1,'30')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('CountdownIntervalInSecs','The interval in seconds of the countdown clock',1,1,'1')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('ViewNbrOfQuestionsTotal','The player can view the number of questions for a game',3,1,'true')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('AnswerSubmitSuccessMessage','The message that is displayed to a player when the answer(s) are accepted by In Common and also when the answer is correct for an LMS game.',0,1,
'The submission of the Game<br/><span class="popupGameName">Game.Name</span><br/> was successful.')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('AnswerSubmitFailMessage','The message that is displayed to a player when the answer(s) are accepted by In Common and also when the answer is incorrect for an LMS game.',0,1,
'The submission of the Game<br/><span class="popupGameName">Game.Name</span><br/> was not successful.')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('SubmitConfirmMessage','The message that is displayed to a player when ready to submit answers to In Common.',0,1,
'You''re about to submit the Game <span class="popupGameName">Game.Name</span>.<p>Are you sure?</p>')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('QuestionTimeWarningMessage','The warning message that is displayed to a player when time is running out to submit an answer(s) to In Common.',0,1,
'There are only <span class="popupGameName">Result.QuestionTimeRemaining</span> seconds left for you to submit your answer.</p>')

INSERT INTO ConfigurationG (Name, Description, DataTypeG, ConfigurationType, Value)
VALUES ('QuestionTimeDeadlineMessage','The message that is displayed to a player when time has run out to submit an answer for the game.',0,1,
'Sorry, but your time has run out to answer the current question. You are no longer standing in the Game <span class="popupGameName">Game.Name</span>.<p>You have taken a seat and will no longer be able to play this game.</p>')


--We are going to remove all GameConfiguration and start a new slate
DELETE FROM GameConfiguration  

