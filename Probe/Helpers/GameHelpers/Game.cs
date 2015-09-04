using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using Probe.DAL;
using ProbeDAL.Models;
using Probe.Models.API;
using Probe.Models.View;
using Probe.Helpers.Exceptions;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Text;
using Probe.Helpers.Mics;
using Probe.Helpers.Validations;
using Probe.Helpers.QuestionHelpers;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http;

namespace Probe.Helpers.GameHelpers
{

    public class ProbeGameQuestionDeadline
    {
        public int QuestionNumber { get; set; }
        public long QuestionId { get; set; }
        public DateTime QuestionStartDT { get; set; }
        public DateTime QuestionDeadlineDT { get; set; }
        public DateTime QuestionWarningDT { get; set; }
    }

    public class ProbeGame
    {
        private ProbeDataContext db = new ProbeDataContext();

        public ProbeDAL.Models.Game Game { get; set; }
        public bool DeviceCanPlayGameOnlyOnce { get; set; }
        public int FindGameTimeCompleteInSecs { get; set; }
        public int FirstQuestionTimeCompleteInSecs { get; set; }
        public int MaxNameLengthPrompted { get; set; }
        public int MinNameLengthPrompted { get; set; }
        public bool PlayerCellPhonePrompted { get; set; }
        public bool PlayerEmailPrompted { get; set; }
        public bool PlayerFirstNamePrompted { get; set; }
        public bool PlayerLastNamePrompted { get; set; }
        public bool PlayerNickNamePrompted { get; set; }
        public bool PlayerSexPrompted { get; set; }
        public int QuestionTimeCompleteInSecs { get; set; }
        public int QuestionTimeSlopeInSecs { get; set; }
        public int QuestionTimeWarningInSecs { get; set; }
        public int ServerClientTimeSyncInSecs { get; set; }
        public bool ViewNbrOfQuestionsTotal { get; set; }
        public IList<ProbeGameQuestionDeadline> ProbeGameQuestionDeadlines { get; set; }

        //Constructor for a Game
        public ProbeGame(ProbeDAL.Models.Game game)
        {
            this.Game = game;

            //Get all configuration properties and values for game
            var gameConfigurations = db.ConfigurationG
                .SelectMany(c => c.GameConfigurations.Where(gc => gc.Game.Id == game.Id).DefaultIfEmpty(),
                (c, gc) =>
                    new GameConfigurationDTO
                    {
                        Id = (gc.Id != null) ? gc.Id : -1,
                        ConfigurationGId = c.Id,
                        GameId = game.Id,
                        GameName = game.Name,
                        Name = c.Name,
                        Description = c.Description,
                        DataTypeG = c.DataTypeG,
                        ConfigurationType = c.ConfigurationType,
                        Value = (gc.Value != null) ? gc.Value : c.Value
                    }
                );

            //Set Game instance properties
            foreach (GameConfigurationDTO gcDTO in gameConfigurations)
            {
                if (gcDTO.Name == "FirstQuestionTimeCompleteInSecs") this.FirstQuestionTimeCompleteInSecs = Convert.ToInt32(gcDTO.Value);
                if (gcDTO.Name == "DeviceCanPlayGameOnlyOnce") this.DeviceCanPlayGameOnlyOnce = Convert.ToBoolean(gcDTO.Value);
                if (gcDTO.Name == "FindGameTimeCompleteInSecs") this.FindGameTimeCompleteInSecs = Convert.ToInt32(gcDTO.Value);
                if (gcDTO.Name == "MaxNameLengthPrompted") this.MaxNameLengthPrompted = Convert.ToInt16(gcDTO.Value);
                if (gcDTO.Name == "MinNameLengthPrompted") this.MinNameLengthPrompted = Convert.ToInt16(gcDTO.Value);
                if (gcDTO.Name == "PlayerCellPhonePrompted") this.PlayerCellPhonePrompted = Convert.ToBoolean(gcDTO.Value);
                if (gcDTO.Name == "PlayerEmailPrompted") this.PlayerEmailPrompted = Convert.ToBoolean(gcDTO.Value);
                if (gcDTO.Name == "PlayerFirstNamePrompted") this.PlayerFirstNamePrompted = Convert.ToBoolean(gcDTO.Value);
                if (gcDTO.Name == "PlayerLastNamePrompted") this.PlayerLastNamePrompted = Convert.ToBoolean(gcDTO.Value);
                if (gcDTO.Name == "PlayerNickNamePrompted") this.PlayerNickNamePrompted = Convert.ToBoolean(gcDTO.Value);
                if (gcDTO.Name == "PlayerSexPrompted") this.PlayerSexPrompted = Convert.ToBoolean(gcDTO.Value);
                if (gcDTO.Name == "QuestionTimeCompleteInSecs") this.QuestionTimeCompleteInSecs = Convert.ToInt32(gcDTO.Value);
                if (gcDTO.Name == "QuestionTimeSlopeInSecs") this.QuestionTimeSlopeInSecs = Convert.ToInt16(gcDTO.Value);
                if (gcDTO.Name == "QuestionTimeWarningInSecs") this.QuestionTimeWarningInSecs = Convert.ToInt32(gcDTO.Value);
                if (gcDTO.Name == "ViewNbrOfQuestionsTotal") this.ViewNbrOfQuestionsTotal = Convert.ToBoolean(gcDTO.Value);
                if (gcDTO.Name == "ServerClientTimeSyncInSecs") this.ServerClientTimeSyncInSecs = Convert.ToInt32(gcDTO.Value);
            }

            //For LMS game we will instantiate a ProbeGameQuestionDeadline that will record each question, and
            //its start,deadline,warning datetime. should ascend up from question 1 to the last question
            if (game.GameType.Name == ProbeConstants.LMSGameType)
            {
                this.ProbeGameQuestionDeadlines = new List<ProbeGameQuestionDeadline>();
                int questionCount = 0;
                int decreaseForSlope = 0;
                int questionTimeCompleteDeltaInSecs = 0;
                DateTime NextQuestionDeadlineDT = game.StartDate.Add(new TimeSpan(0, 0, this.FindGameTimeCompleteInSecs));
                foreach (GameQuestion gq in game.GameQuestions.OrderBy(gq => gq.OrderNbr))
                {
                    questionCount++;
                    ProbeGameQuestionDeadline pgdl = new ProbeGameQuestionDeadline();
                    pgdl.QuestionNumber = questionCount;
                    pgdl.QuestionId = gq.QuestionId;

                    //calculate total decrease in seconds for slope. Slope only comes into play on the third question
                    //and beyond
                    if (questionCount == 1)
                    {
                        decreaseForSlope = 0;
                        questionTimeCompleteDeltaInSecs = this.FirstQuestionTimeCompleteInSecs;
                    }
                    else if (questionCount == 2)
                    {
                        decreaseForSlope = 0;
                        questionTimeCompleteDeltaInSecs = this.QuestionTimeCompleteInSecs;
                    }
                    else
                    {
                        decreaseForSlope += this.QuestionTimeSlopeInSecs;
                        questionTimeCompleteDeltaInSecs = this.QuestionTimeCompleteInSecs - decreaseForSlope;
                    }

                    TimeSpan deltaToNextQuestionInSecs = new TimeSpan(0, 0, questionTimeCompleteDeltaInSecs);

                    pgdl.QuestionDeadlineDT = NextQuestionDeadlineDT.Add(deltaToNextQuestionInSecs);

                    //For the first question, the question start date goes all the way back to game start date
                    if (questionCount == 1)
                    {
                        pgdl.QuestionStartDT = game.StartDate;
                    }
                    else
                    {
                        pgdl.QuestionStartDT = pgdl.QuestionDeadlineDT.Subtract(new TimeSpan(0, 0, questionTimeCompleteDeltaInSecs));
                    }


                    pgdl.QuestionWarningDT = pgdl.QuestionDeadlineDT.Subtract(new TimeSpan(0, 0, this.QuestionTimeWarningInSecs));

                    this.ProbeGameQuestionDeadlines.Add(pgdl);

                    NextQuestionDeadlineDT = NextQuestionDeadlineDT.Add(deltaToNextQuestionInSecs);


                }//foreach (GameQuestion gq in game.GameQuestions.OrderBy(gq => gq.OrderNbr))

            }//if (game.GameType.Name == ProbeConstants.LMSGameType)

        }//  public ProbeGame(Game game)

        public string GameType
        {
            get
            {
                return this.Game.GameType.Name;
            }
        }

        public bool IsActive()
        {

            bool status = false;
            if (((DateTime.Compare(DateTime.UtcNow, this.Game.StartDate) > 0 &&
                                DateTime.Compare(DateTime.UtcNow, this.Game.EndDate) <= 0)
                                || (this.Game.Players.Count() > 0))
                                && !this.Game.SuspendMode)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }//public bool IsGameActive()

        public bool IsPlayerSubmitted(Player player)
        {
            bool status = false;
            if (player.Id != 0)
            {
                if (Game.Players.Where(p => p.Id == player.Id).Count() > 0)
                {
                    status = true;
                }
            }
            return status;
        }//public bool IsPlayerSubmitted(Player player)

        public bool IsValidGameAnswer(ICollection<GameAnswer> gameAnswers)
        {
            if (gameAnswers.Count() == 0)
            {
                //If there are no questions answered; then there is a problem
                return false;
            }
            else if (this.GameType == ProbeConstants.LMSGameType && gameAnswers.Count() != 1)
            {
                //if the game is LMS then answers should be coming one at a time
                return false;
            }

            return true;
        }//public bool IsValidGameAnswer(ICollection<GameAnswer> gameAnswers)

        public void ValidateGamePlayer (Player player)
        {
            //we don't have to do any validation if the game is LMS and the player id is already set. This
            //just means that the player has already submitted something and we have the player registered.
            if (this.GameType == ProbeConstants.LMSGameType && IsPlayerSubmitted(player)) return; 

            //determine if there is another player with the same name that has already submitted for a game play
            //} else if (firstName == "" && lastName == "" && nickName == "" && email != "") {
            //    return email; //last hope

            int recordCount = 0;
            if (player.FirstName != null && player.LastName != null)
            {
                recordCount = db.Player.Where(p => p.GameId == this.Game.Id
                              && p.FirstName == player.FirstName
                              && p.LastName == player.LastName).Count();
            }
            else if (player.FirstName == null && player.NickName != null && player.LastName != null)
            {
                recordCount = db.Player.Where(p => p.GameId == this.Game.Id
                              && p.NickName == player.NickName
                              && p.LastName == player.LastName).Count();
            }
            else if (player.FirstName != null && player.NickName != null && player.LastName == null)
            {
                recordCount = db.Player.Where(p => p.GameId == this.Game.Id
                              && p.FirstName == player.FirstName
                              && p.NickName == player.NickName).Count();
            }
            else if (player.FirstName != null && player.NickName == null && player.LastName == null)
            {
                recordCount = db.Player.Where(p => p.GameId == this.Game.Id
                              && p.FirstName == player.FirstName).Count();
            }
            else if (player.FirstName == null && player.NickName == null && player.LastName != null)
            {
                recordCount = db.Player.Where(p => p.GameId == this.Game.Id
                              && p.LastName == player.LastName).Count();
            }
            else if (player.FirstName == null && player.NickName != null && player.LastName == null)
            {
                recordCount = db.Player.Where(p => p.GameId == this.Game.Id
                              && p.NickName == player.NickName).Count();
            }
            else if (player.FirstName == null && player.NickName == null && player.LastName == null && player.EmailAddr != null)
            {
                recordCount = db.Player.Where(p => p.GameId == this.Game.Id
                              && p.EmailAddr == player.EmailAddr).Count();
            }
            else
            {
                throw new GameInvalidPlayerNameException();
            }

            
            if (recordCount > 0)
            {
                throw new GameDuplicatePlayerNameException();
            }

            if (player.FirstName != null)
            {
                if (player.FirstName.Length < this.MinNameLengthPrompted || player.FirstName.Length > this.MaxNameLengthPrompted)
                {
                    throw new GameInvalidFirstNameException();
                }
            }

            if (player.NickName != null)
            {
                if (player.NickName.Length < this.MinNameLengthPrompted || player.NickName.Length > this.MaxNameLengthPrompted)
                {
                    throw new GameInvalidNickNameException();
                }
            }

            if (player.LastName != null)
            {
                if (player.LastName.Length < this.MinNameLengthPrompted || player.LastName.Length > this.MaxNameLengthPrompted)
                {
                    throw new GameInvalidLastNameException();
                }
            }

        }//ValidateGamePlayerActive

        public int NbrPlayers
        {
            get
            {
                return db.Game.Find(Game.Id).Players.Count();
            }
        }

        public int NbrPlayersActive
        {
            get
            {
                return db.Game.Find(Game.Id).Players.Where(p => p.Active).Count();
            }
        }

        public bool IsPlayerHaveAnswers(Player player)
        {
            return db.Player.Find(player.Id).GameAnswers.Count() > 0; 
        }

        //returns the number of answers correct from the GameAnswers object
        public int NbrPlayerAnswersCorrect(ICollection<GameAnswerDTO> gameAnswersDTO)
        {
            int nbrAnswersCorrect = 0;

            //We can correct if it's the Test or LMS game
            if (this.GameType == ProbeConstants.TestGameType || this.GameType == ProbeConstants.LMSGameType)
            {
                var questionsAndTheirCorrectChoices = db.ChoiceQuestion
                    .Where(cq => cq.GameQuestions.Any(gq => gq.GameId == Game.Id))
                    .Select(cq => new
                    {
                        QuestionId = cq.Id,
                        ChoiceId = cq.Choices.Where(c => c.Correct).FirstOrDefault().Id
                    });

                //bool isAnyAnswersInCorrect = gameAnswersDTO.Any(ga => questionsAndTheirCorrectChoices.Any(cq => cq.QuestionId == ga.QuestionId && cq.ChoiceId != ga.ChoiceId));
                //int nbrAnswersInCorrect = gameAnswersDTO.Where(ga => questionsAndTheirCorrectChoices.Any(cq => cq.QuestionId == ga.QuestionId && cq.ChoiceId != ga.ChoiceId)).Count();

                nbrAnswersCorrect = gameAnswersDTO.Where(ga => questionsAndTheirCorrectChoices.Any(cq => cq.QuestionId == ga.QuestionId && cq.ChoiceId == ga.ChoiceId)).Count();
            }
            return nbrAnswersCorrect;
        
        }//NbrPlayerAnswersCorrect(ICollection<GameAnswerDTO> gameAnswersDTO)

        /*
         * Check the date/time deadline of the last question in the GameAnswers queue and see if the submission
         * was HTTP Putted (uploaded) on time. Will return true if the submission is ontime. Will return false if not.
         * Will ALWAYS return true if NOT an LMS game. NOTE: NEED TO MAKE THIS GAME CLASS A FACTORY
         */
        public bool IsPlayerGameAnswerOnTime(DateTime dateSubmission, ICollection<GameAnswer> gameAnswers)
        {
            if (this.GameType != ProbeConstants.LMSGameType)
            {
                return true;
            } 
            else
            {
                long lastQuestionId = db.Choice.Find(gameAnswers.LastOrDefault().ChoiceId).ChoiceQuestionId;

                DateTime questionAnswerDeadline = this.ProbeGameQuestionDeadlines.Where(pgqd => pgqd.QuestionId == lastQuestionId)
                    .FirstOrDefault().QuestionDeadlineDT;

                return questionAnswerDeadline >= dateSubmission;
            }

        }//public bool IsPlayerGameAnswerOnTime(DateTime dateSubmission, ICollection<GameAnswer> gameAnswers)

        /*
         * Check the date/time start of the last question in the GameAnswers queue and see if the submission
         * was HTTP Putted (uploaded) not too early. Will return true if the submission is not tool early. Will return false if submission
         * is too early.
         * Will ALWAYS return true if NOT an LMS game. NOTE: NEED TO MAKE THIS GAME CLASS A FACTORY
         */
        public bool IsPlayerGameAnswerNotTooEarly(DateTime dateSubmission, ICollection<GameAnswer> gameAnswers)
        {
            if (this.GameType != ProbeConstants.LMSGameType)
            {
                return true;
            }
            else
            {
                long lastQuestionId = db.Choice.Find(gameAnswers.LastOrDefault().ChoiceId).ChoiceQuestionId;

                DateTime questionAnswerStart = this.ProbeGameQuestionDeadlines.Where(pgqd => pgqd.QuestionId == lastQuestionId)
                    .FirstOrDefault().QuestionStartDT;

                return questionAnswerStart <= dateSubmission;
            }

        }//public bool IsPlayerGameAnswerNotTooEarly(DateTime dateSubmission, ICollection<GameAnswer> gameAnswers)

        public bool IsPlayerActive(Player player)
        {
            return db.Player.Find(player.Id).Active;
        }

        public void SetPlayerStatus(Player player, bool activeStatus, Player.PlayerGameReasonType playerGameReason)
        {
            //We need to make the player inactive.
            Player playerUpdate = db.Player.Find(player.Id);
            playerUpdate.Active = activeStatus;
            playerUpdate.PlayerGameReason = playerGameReason;
            db.Entry(playerUpdate).State = EntityState.Modified;
            db.SaveChanges();
        }//public void SetPlayerStatus(Player player, bool activeStatus)

        /*
         * Get the question (id) that is the most recent to have gone passed the deadline
         */
        public long GetMostRecentQuestionPassedDeadline(DateTime dateTimeNow)
        {
            long questionId = ProbeConstants.NoPrimaryKeyId;

            IList<ProbeGameQuestionDeadline> probeGameQuestionDeadlinesSortedByDeadlineDESC =
                this.ProbeGameQuestionDeadlines.OrderByDescending(pgqd => pgqd.QuestionDeadlineDT).ToList();

            foreach (ProbeGameQuestionDeadline pgqd in probeGameQuestionDeadlinesSortedByDeadlineDESC)
            {
                if (dateTimeNow > pgqd.QuestionDeadlineDT)
                {
                    questionId = pgqd.QuestionId;
                }

                if (questionId != ProbeConstants.NoPrimaryKeyId) break; //we have our question; so let's get out of the loop
            }

            return questionId;
        }// public long GetMostRecentQuestionPassedDeadline(DateTime dateTimeNow)

        public ProbeGameQuestionDeadline GetProbeGameQuestionDeadline(long questionId)
        {
            return this.ProbeGameQuestionDeadlines.Where(pgqd => pgqd.QuestionId == questionId).FirstOrDefault();
        }

        public int GetMaxQuestionNbrSubmitted()
        {
            return db.Database.SqlQuery<int>("exec GetMaxQuestionNbrSubmitted " + Game.Id).FirstOrDefault();
        }

        /*
         * Return a dictionary (id, boolean) that specifies the player id and true/false on whether they have
         * answered the most recent question that the deadline has passed
         */
        public Dictionary<long, bool> GetAllActivePlayersHasAnsweredQuestion(long questionId)
        {
            ProbeDataContext db = new ProbeDataContext();

            return db.Player.Where(p => p.Active && p.GameId == this.Game.Id)
                .Select(p => new
                {
                    Id = p.Id,
                    Answered = p.GameAnswers.Any(ga => ga.Choice.ChoiceQuestionId == questionId)
                }).ToDictionary(p => p.Id, p => p.Answered);
        }//public Dictionary<long, bool> GetAllActivePlayersHasAnsweredQuestion(long questionId)


        public static Game CloneGameFromAnotherUser(Controller controller, ProbeDataContext db, long sourceGameId, string destAspNetUsersId)
        {
            try
            {

                //Clone the game to be owned to the destination user. This function will clone
                //game, gameconfiguration, gamequestion, question, choicequestion, and choice
                bool gamePlayInd = true;
                bool cloneCrossUsers = true;
                Game game = CloneGame(controller, db, sourceGameId, destAspNetUsersId, cloneCrossUsers, gamePlayInd);

                return game;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /*
         * Will clone the game and all artifacts associated with that game. gameconfiguration, gamequestion, question/choicequestion,
         * choice and game records. The game to clone is defined by the sourceGameId. Where the game is clone to (what user) is 
         * determined by the destAspNetUsersId
         */
        public static Game CloneGame(Controller controller, ProbeDataContext db, long sourceGameId
                                    , string destAspNetUsersId, bool cloneCrossUsers, bool gamePlayInd)
        {

            //clone game - always in suspend mode so its not playable yet and it's editable
            Game gSource = db.Game.Find(sourceGameId);
            string newName = GetClonedGameName(gSource.Name, destAspNetUsersId);
            string newCode = GetClonedCode(gSource.Code);

            Game gNew = new Game
            {
                AspNetUsersId = destAspNetUsersId,
                GameTypeId = gSource.GameTypeId,
                Name = newName,
                Description = gSource.Description,
                Code = newCode,
                TestMode = gSource.TestMode,
                Published = (cloneCrossUsers) ? gSource.Published : false,
                SuspendMode = (cloneCrossUsers) ? gSource.SuspendMode : false,
                ClientReportAccess = gSource.ClientReportAccess,
                StartDate = gSource.StartDate,
                EndDate = gSource.EndDate,
                GameUrl = gSource.GameUrl,
                ACLId = gSource.ACLId
            };

            db.Game.Add(gNew);
            db.SaveChanges(controller.Request != null ? controller.Request.LogonUserIdentity.Name : null); //this should get us a new Game Id
            long clonedGameId = gNew.Id;

            //clone all gameconfigurations. should only pull the gameconfiguration records that exist. Any configuration that is still
            //using the default ConfigurationG record will not be pulled here. No need to clone this record.
            IList<GameConfigurationDTO> gameConfigurations = db.ConfigurationG
                .Where(c => c.ConfigurationType != ConfigurationG.ProbeConfigurationType.GLOBAL)
                .SelectMany(c => c.GameConfigurations.Where(gc => gc.Game.Id == sourceGameId),
                (c, gc) =>
                    new GameConfigurationDTO
                    {
                        ConfigurationGId = gc.ConfigurationGId,
                        Value = gc.Value
                    }
                ).ToList();

            foreach (GameConfigurationDTO gameConfiguration in gameConfigurations)
            {
                GameConfiguration clonedGameConfiguration = new GameConfiguration
                {
                    GameId = clonedGameId,
                    ConfigurationGId = gameConfiguration.ConfigurationGId,
                    Value = gameConfiguration.Value
                };

                db.GameConfiguration.Add(clonedGameConfiguration);
                db.SaveChanges(controller.Request != null ? controller.Request.LogonUserIdentity.Name : null); //this should get us a new GameQuestion Id

            }//foreach (GameConfiguration gameConfiguration in gameConfigurations)

            //clone gamequestions and question/choices for each gamequestion
            IList<GameQuestion> gameQuestions = db.GameQuestion.Where(gq => gq.GameId == sourceGameId).ToList();

            Dictionary<long, Dictionary<long, long>> questionXreference = new Dictionary<long, Dictionary<long, long>>();
            foreach (GameQuestion gameQuestion in gameQuestions)
            {

                //attach questions to game
                long sourceQuestionId = gameQuestion.QuestionId;
                Dictionary<long, long> choiceXreference = new Dictionary<long, long>();
                long clonedQuestionId = ProbeQuestion.CloneQuestion(controller, db, true, sourceQuestionId, ref choiceXreference);

                GameQuestion clonedGameQuestion = new GameQuestion
                {
                    GameId = clonedGameId,
                    QuestionId = clonedQuestionId,
                    OrderNbr = gameQuestion.OrderNbr,
                    Weight = gameQuestion.Weight
                };

                db.GameQuestion.Add(clonedGameQuestion);
                db.SaveChanges(controller.Request != null ? controller.Request.LogonUserIdentity.Name : null); //this should get us a new GameQuestion Id

                //We are building a question - choice cross reference table for down the road (if needed). old question id -> (old choice id -> new choice id)
                questionXreference.Add(gameQuestion.QuestionId, choiceXreference);

            }//foreach (GameQuestion gameQuestion in gameQuestions)

            //if directed, then the game that is played will be cloned. Players, GameAnswers
            if (gamePlayInd)
            {
                //Get all the players for the game played
                IList<Player> players = db.Player.Where(p => p.GameId == sourceGameId).ToList();
                foreach (Player player in players)
                {
                    Player clonedPlayer = new Player
                    {
                        LastName = player.LastName,
                        FirstName = player.FirstName,
                        MiddleName = player.MiddleName,
                        NickName = player.NickName,
                        EmailAddr = player.EmailAddr,
                        MobileNbr = player.MobileNbr,
                        Sex = player.Sex,
                        SubmitDate = player.SubmitDate,
                        SubmitTime = player.SubmitTime,
                        GameId = gNew.Id,
                        Active = player.Active,
                        PlayerGameReason = player.PlayerGameReason
                    };
                    db.Player.Add(clonedPlayer);
                    db.SaveChanges(controller.Request != null ? controller.Request.LogonUserIdentity.Name : null); //this should get us a new Player Id

                    //Get all the OLD answers for the player in this iteration
                    IList<GameAnswer> gameanswers = db.GameAnswer.Where(ga => ga.PlayerId == player.Id).ToList();
                    foreach (GameAnswer gameAnswer in gameanswers)
                    {

                        GameAnswer clonedGameAnswer = new GameAnswer
                        {
                            PlayerId = clonedPlayer.Id,
                            //gameAnswer.Choice.ChoiceQuestionId = old questionId
                            ChoiceId = questionXreference[gameAnswer.Choice.ChoiceQuestionId][gameAnswer.ChoiceId]
                        };

                        db.GameAnswer.Add(clonedGameAnswer);
                        db.SaveChanges(controller.Request != null ? controller.Request.LogonUserIdentity.Name : null); //this should get us a new GameAnswer Id

                    }//foreach (GameAnswer gameAnswer in gameanswers)

                }//foreach (Player player in players)

            }//if (gamePlayInd)

            return gNew;
        }//CloneGame

        private static string GetClonedGameName(string name, string AspNetUsersId)
        {
            string newName = "";
            string newNameLastBeforeToLong = "";
            int copyNumber = 0;
            string copyString = " COPY";

            /*
             * Let's just try and add the string "COPY" to the end of the current name to start with
             */
            newNameLastBeforeToLong = name.Trim();
            newName = name.Trim(); // +copyString; (don't think we need this)
            while (ProbeValidate.IsGameNameExistForUser(newName, AspNetUsersId) && (newName.Length < ProbeConstants.QuestionNameMaxChars))
            {
                newNameLastBeforeToLong = newName;
                newName = newName + copyString;
            };

            /*
             * If we get here and the new name doesn't exist and it's less than equal to the max characters,
             * THEN we have our new question name. Otherwise, we go to approach II
             */
            if (ProbeValidate.IsGameNameExistForUser(newName, AspNetUsersId) || (newName.Length > ProbeConstants.QuestionNameMaxChars))
            {
                newName = newNameLastBeforeToLong;
                do
                {
                    string copyNewString = " COPY";
                    if (copyNumber++ != 0)
                    {
                        copyNewString = copyString.Substring(0, 5 - copyNumber.ToString().Length) + copyNumber.ToString();
                    }

                    newName = newName.Substring(0, ProbeConstants.GameNameMaxChars - 5) + copyNewString;


                } while (ProbeValidate.IsGameNameExistForUser(newName, AspNetUsersId));


            }

            return newName;
        }//GetClonedGameName

        private static string GetClonedCode(string name)
        {
            string newName = "";
            string newNameLastBeforeToLong = "";
            int copyNumber = 0;
            string copyString = " COPY";

            /*
             * Let's just try and add the string "COPY" to the end of the current name to start with
             */
            newNameLastBeforeToLong = name.Trim();
            newName = name.Trim() + copyString;
            while (ProbeValidate.IsCodeExistInProbe(newName) && (newName.Length < ProbeConstants.GameCodeMaxChars))
            {
                newNameLastBeforeToLong = newName;
                newName = newName + copyString;
            };

            /*
             * If we get here and the new name doesn't exist and it's less than equal to the max characters,
             * THEN we have our new question name. Otherwise, we go to approach II
             */
            if (ProbeValidate.IsCodeExistInProbe(newName) || (newName.Length > ProbeConstants.GameCodeMaxChars))
            {
                newName = newNameLastBeforeToLong;
                do
                {
                    string copyNewString = " COPY";
                    if (copyNumber++ != 0)
                    {
                        copyNewString = copyString.Substring(0, 5 - copyNumber.ToString().Length) + copyNumber.ToString();
                    }

                    newName = newName.Substring(0, ProbeConstants.GameNameMaxChars - 5) + copyNewString;


                } while (ProbeValidate.IsCodeExistInProbe(newName));


            }

            return newName;
        }//GetClonedCodeName

        /*
         * Will delete the game and all artifacts associated with that game. gameconfiguration, gamequestion, question/choicequestion,
         * choice and game records.
         */
        public static void DeleteGame(Controller controller, ProbeDataContext db, long gameId)
        {

            Game g = db.Game.Find(gameId);

            //remove all questions attached to the game/gamequestions
            IList<GameQuestion> gameQuestions = db.GameQuestion.Where(gq => gq.GameId == gameId).ToList();
            foreach (GameQuestion gameQuestion in gameQuestions)
            {
                ProbeQuestion.DeleteQuestion(controller, db, gameQuestion.QuestionId);
            }

            db.Player.RemoveRange(g.Players); //Remove all players that have submitted games
            db.GameQuestion.RemoveRange(g.GameQuestions); //Remove all gamequestions for game
            db.GameConfiguration.RemoveRange(g.GameConfigurations);
            db.Game.Remove(g); //Remove the game itself
            db.SaveChanges(controller.Request != null ? controller.Request.LogonUserIdentity.Name : null);

        }//DeleteGame

        //Delete player - if the player Id passed is valid. Will delete game answers if they exist.
        public static void DeletePlayer(ProbeDataContext db, Player player)
        {
            //Only delete if the player Id is valid. Otherwise, we ignore this.
            if (player.Id > 0)
            {
                var gpas = db.GameAnswer.Where(ga => ga.PlayerId == player.Id);
                foreach (GameAnswer ga in gpas)
                {
                    db.GameAnswer.Remove(ga);
                }
                db.Player.Remove(player);

                db.SaveChanges();
            }

        }//public void DeletePlayer()

        public static void UpdateAllGamePlayerStatuses()
        {
            DateTime dateTimeNowUTC = DateTime.UtcNow;

            IList<Game> activeLMSGames = ProbeValidate.GetAllActiveLMSGamesWithActivePlayers();
            foreach (Game game in activeLMSGames)
            {
                try
                {
                    UpdateAllGamePlayerStatuses(game, dateTimeNowUTC);
                }
                catch (Exception ex)
                {
                    throw ex; //throw it up to the calling program
                }

            }//foreach (Game game in activeLMSGames)

        }//public static void UpdateAllGameStatuses()

        public static void UpdateAllGamePlayerStatuses(Game game, DateTime dateTimeNowUTC)
        {
            ProbeDataContext db = new ProbeDataContext();

            ProbeGame probeGame = new ProbeGame(game);

            //Determine the most recent question's deadline for each game
            long questionIdForMostRecentDeadlinePassed = probeGame.GetMostRecentQuestionPassedDeadline(dateTimeNowUTC);

            //If there are no questions that have recently passed the deadline, then there is no status updates to make.
            if (questionIdForMostRecentDeadlinePassed == ProbeConstants.NoPrimaryKeyId) return;

            try
            {
                //Go to the Database and get the Game - Questions - Choices All at Once Time
                db.Database.ExecuteSqlCommand("exec SetPlayersOfGameNotSubmittedToInActive " + probeGame.Game.Id + "," + questionIdForMostRecentDeadlinePassed);
            }
            catch (Exception ex)
            {
                throw ex; //throw it up to the calling program
            }

            //THE C# CODE BELOW WORKS AND WILL SET THE APPROPRIATE PLAYERS FOR THE GAME INACTIVE.
            //HOWEVER, MNS FELT THAT THE SP WOULD BE MORE EFFICIENT
            ////Determine if each player of the game has submitted answer to the most recent question
            //Dictionary<long, bool> dctActivePlayerHasAnsweredQuestion
            //    = probeGame.GetAllActivePlayersHasAnsweredQuestion(questionIdForMostRecentDeadlinePassed);

            ////If the player doesn't have a submitted answer to the most recent question THEN set that player to INACTIVE
            //foreach (var playerInfo in dctActivePlayerHasAnsweredQuestion)
            //{

            //    //Check if this player has answered the question (questionId = questionIdForMostRecentDeadlinePassed)
            //    if (!playerInfo.Value)
            //    {
            //        //If player hasn't answered then we are going to set the player status to inactive.
            //        Player player = db.Player.Find(playerInfo.Key);
            //        probeGame.SetPlayerStatus(player, false);

            //    }

            //}//foreach (var playerInfo in dctActivePlayerHasAnsweredQuestion) {


        }//public static void UpdateAllGameStatuses(Game game, DateTime dateTimeNowUTC)

    }
}