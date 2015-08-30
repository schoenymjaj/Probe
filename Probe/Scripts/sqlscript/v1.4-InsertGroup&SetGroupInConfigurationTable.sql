

INSERT INTO [Group] ([Name],Description) VALUES('Player Messages', 'Messages that are displayed to a player that is play a game')
INSERT INTO [Group] ([Name],Description) VALUES('Player Name Prompts', 'Fields that will be entered by a player to identify them')
INSERT INTO [Group] ([Name],Description) VALUES('Question and Answer Schedule', 'Fields that will calculate when questions for an LMS game will be available')
INSERT INTO [Group] ([Name],Description) VALUES('In Common Cache', 'In Common cache properties that affect all player''s response time')
INSERT INTO [Group] ([Name],Description) VALUES('In Common Documentation', 'In Common documentation')
INSERT INTO [Group] ([Name],Description) VALUES('Player Devices', 'Fields to characterize what a player can do on her device')
INSERT INTO [Group] ([Name],Description) VALUES('Game Display', 'Fields to characterize what the player views')


UPDATE ConfigurationG SET GroupId = 6, ShortDescription = 'A Device Can Play Game Only Once' WHERE id = 1
UPDATE ConfigurationG SET GroupId = 4, ShortDescription = 'Time that In Common Will Cache' WHERE id = 2
UPDATE ConfigurationG SET GroupId = 2, ShortDescription = 'Min Characters For Player Prompt' WHERE id = 3
UPDATE ConfigurationG SET GroupId = 2, ShortDescription = 'Max Characters For Player Prompt' WHERE id = 4
UPDATE ConfigurationG SET GroupId = 2, ShortDescription = 'Player First Name Prompted' WHERE id = 5
UPDATE ConfigurationG SET GroupId = 2, ShortDescription = 'Player Nick Name Prompted' WHERE id = 6
UPDATE ConfigurationG SET GroupId = 2, ShortDescription = 'Player Sex Prompted' WHERE id = 7
UPDATE ConfigurationG SET GroupId = 2, ShortDescription = 'Player Last Name Prompted' WHERE id = 8
UPDATE ConfigurationG SET GroupId = 2, ShortDescription = 'Player Email Prompted' WHERE id = 9
UPDATE ConfigurationG SET GroupId = 2, ShortDescription = 'Player Cell Phone Prompted' WHERE id = 10
UPDATE ConfigurationG SET GroupId = 3, ShortDescription = 'Time Player Has To Find Game' WHERE id = 11
UPDATE ConfigurationG SET GroupId = 3, ShortDescription = 'Time Limit For First Question To Be Answered' WHERE id = 12
UPDATE ConfigurationG SET GroupId = 3, ShortDescription = 'Time Limit For All Questions To Be Answered' WHERE id = 13
UPDATE ConfigurationG SET GroupId = 3, ShortDescription = 'Time Before Question Deadline for Warning Msg' WHERE id = 14
UPDATE ConfigurationG SET GroupId = 3, ShortDescription = 'Time Reduced To Answer Each Ensuing Question ' WHERE id = 15
UPDATE ConfigurationG SET GroupId = 7, ShortDescription = 'Nbr of Total Questions Displayed During Game' WHERE id = 16
UPDATE ConfigurationG SET GroupId = 1, ShortDescription = 'Message After Player Successfully Submits' WHERE id = 17
UPDATE ConfigurationG SET GroupId = 1, ShortDescription = 'Message After Player Unsuccessfully Submits' WHERE id = 18
UPDATE ConfigurationG SET GroupId = 1, ShortDescription = 'Message To Confirm If Player Will Submit' WHERE id = 19
UPDATE ConfigurationG SET GroupId = 1, ShortDescription = 'Warning Message As Question Deadline Approaching' WHERE id = 20
UPDATE ConfigurationG SET GroupId = 1, ShortDescription = 'Message After Question Deadline Is Reached' WHERE id = 21
UPDATE ConfigurationG SET GroupId = 5, ShortDescription = 'In Common About Documentation' WHERE id = 22
UPDATE ConfigurationG SET GroupId = 3, ShortDescription = 'Time Range For In Common and Device To Be In-Sync' WHERE id = 23
UPDATE ConfigurationG SET GroupId = 3, ShortDescription = 'Time Of The Question Countdown Clock' WHERE id = 24

