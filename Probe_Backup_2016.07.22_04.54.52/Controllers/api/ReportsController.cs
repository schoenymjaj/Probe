using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Probe.DAL;
using ProbeDAL.Models;
using Probe.Models.API;
using Probe.Helpers.Validations;
using Probe.Helpers.Exceptions;
using Probe.Helpers.GameHelpers;
using Probe.Helpers.Mics;

namespace Probe.Controllers.api
{
    public class ReportsController : ApiController
    {
        private ProbeDataContext db = new ProbeDataContext();

        [Route("api/Reports/GetGamePlayerMatchMinMaxData/{gameid}/{code}")]
        public List<GamePlayerMatchMinMaxReturn> GetGamePlayerMatchMinMaxData(long gameid, string code)
        {
            /*
            the gameid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameid, code);

            var result = db.Database.SqlQuery<GamePlayerMatchMinMaxData>
                                             ("exec GetGamePlayerMatchMinMax " + gameid).ToList();

            List<GamePlayerMatchMinMaxReturn> reportData = new List<GamePlayerMatchMinMaxReturn>();
            foreach (GamePlayerMatchMinMaxData row in result)
            {

                GamePlayerMatchMinMaxReturn gpmmmr = new GamePlayerMatchMinMaxReturn
                {
                    PlayerId = row.PlayerId,
                    PlayerName = row.PlayerName,
                    MaleMaxNbrOfMatched = row.MaleMaxNbrOfMatched,
                    MaleMaxMatchedPlayerId = row.MaleMaxMatchedPlayerId,
                    MaleMaxPlayerName = row.MaleMaxPlayerName,
                    MaleMinNbrOfMatched = row.MaleMinNbrOfMatched,
                    MaleMinMatchedPlayerId = row.MaleMinMatchedPlayerId,
                    MaleMinPlayerName = row.MaleMinPlayerName,
                    FemaleMaxNbrOfMatched = row.FemaleMaxNbrOfMatched,
                    FemaleMaxMatchedPlayerId = row.FemaleMaxMatchedPlayerId,
                    FemaleMaxPlayerName = row.FemaleMaxPlayerName,
                    FemaleMinNbrOfMatched = row.FemaleMinNbrOfMatched,
                    FemaleMinMatchedPlayerId = row.FemaleMinMatchedPlayerId,
                    FemaleMinPlayerName = row.FemaleMinPlayerName,


                };
                reportData.Add(gpmmmr);

            }

            return reportData;
        }

        [Route("api/Reports/GetPlayerMatchSummaryData/{gameid}/{code}/{playerid}")]
        public List<PlayerMatchSummaryReturn> GetPlayerMatchSummaryData(long gameid, string code, long playerid)
        {
            /*
            the gameid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameid, code);

            var result = db.Database.SqlQuery<PlayerMatchSummaryData>
                                             ("exec GetPlayerMatchSummary " + gameid + "," + playerid).ToList();

            List<PlayerMatchSummaryReturn> reportData = new List<PlayerMatchSummaryReturn>();
            foreach (PlayerMatchSummaryData row in result)
            {

                PlayerMatchSummaryReturn pmsr = new PlayerMatchSummaryReturn
                {
                    Id = row.MatchedPlayerId,
                    Name = row.MatchedPlayerName,
                    Value = row.NbrOfMatched
                };
                reportData.Add(pmsr);

            }

            return reportData;
        }

        [Route("api/Reports/GetPlayerMatchDetailData/{gameid}/{code}/{playerid}/{matchedplayerid}")]
        public List<PlayerMatchDetailReturn> GetPlayerMatchDetailData(long gameid, string code, long playerid, long matchedplayerid)
        {
            /*
            the gameid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameid, code);

            int filterType = 0; //get all questions match or no-match

            var result = db.Database.SqlQuery<PlayerMatchDetailData>
                                             ("exec GetPlayerMatchDetail " + gameid + "," + playerid + ","
                                             + matchedplayerid + "," + filterType + ",'order by OrderNbr asc'").ToList();


            List<PlayerMatchDetailReturn> reportData = new List<PlayerMatchDetailReturn>();
            foreach (PlayerMatchDetailData row in result)
            {
                PlayerMatchDetailReturn pmdr = new PlayerMatchDetailReturn
                {
                    PlayerName = row.PlayerName,
                    MatchedPlayerId = row.MatchedPlayerId,
                    MatchedPlayerName = row.MatchedPlayerName,
                    QuestionId = row.QuestionId,
                    Question = row.Question,
                    PlayerChoice = row.PlayerChoice,
                    MatchedPlayerChoice = row.MatchedPlayerChoice,
                    Match = row.Match,
                    PercentChosen = row.PercentChosen
                };

                reportData.Add(pmdr);
            }

            return reportData;
        }

        [Route("api/Reports/GetPlayerTestSummaryData/{gameid}/{code}")]
        public List<PlayerTestSummaryReturn> GetPlayerTestSummaryData(long gameid, string code)
        {
            /*
            the gameid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameid, code);

            var result = db.Database.SqlQuery<PlayerTestSummaryData>
                                             ("exec GetPlayerTestSummary " + gameid).ToList();

            List<PlayerTestSummaryReturn> reportData = new List<PlayerTestSummaryReturn>();
            foreach (PlayerTestSummaryData row in result)
            {

                PlayerTestSummaryReturn ptsr = new PlayerTestSummaryReturn
                {
                    Id = row.PlayerId,
                    Name = row.PlayerName,
                    Value = row.PercentCorrect
                };
                reportData.Add(ptsr);

            }

            return reportData;
        }

        [Route("api/Reports/GetPlayerTestDetailData/{gameid}/{code}/{playerid}")]
        public List<PlayerTestDetailReturn> GetPlayerTestDetailData(long gameid, string code, long playerid)
        {
            /*
            the gameid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameid, code);

            var result = db.Database.SqlQuery<PlayerTestDetailData>
                                             ("exec GetPlayerTestDetail " + gameid + "," + playerid).ToList();


            List<PlayerTestDetailReturn> reportData = new List<PlayerTestDetailReturn>();
            foreach (PlayerTestDetailData row in result)
            {
                PlayerTestDetailReturn ptdr = new PlayerTestDetailReturn
                {
                    PlayerName = row.PlayerName,
                    QuestionId = row.QuestionId,
                    Question = row.Question,
                    OrderNbr = row.OrderNbr,
                    SelectedChoices = row.SelectedChoices,
                    CorrectChoices = row.CorrectChoices,
                    QuestionCorrect = row.QuestionCorrect,
                    PercentCorrect = row.PercentCorrect
                };

                reportData.Add(ptdr);
            }

            return reportData;
        }

        [Route("api/Reports/GetGameLMSSummaryData/{gameid}/{code}")]
        public GameLMSSummaryReturn GetGameLMSSummaryData(long gameid, string code)
        {
            /*
            the gameid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameid, code);

            try
            {
                DateTime dateTimeNowUTC = DateTime.UtcNow;

                var db = new ProbeDataContext();
                Game game = db.Game.Find(gameid);
                ProbeGame probeGame = new ProbeGame(game);

                //update player statuses for a game.
                ProbeGame.UpdateAllGamePlayerStatuses(game, dateTimeNowUTC);


                //Get information about question that most recently passed the deadline and the next question after that
                long questionIdForMostRecentQuestion = probeGame.GetMostRecentQuestionPassedDeadline(dateTimeNowUTC);
                if (questionIdForMostRecentQuestion != ProbeConstants.NoPrimaryKeyId)
                {

                    int mostRecentQuestionNbrPassed = probeGame.GetProbeGameQuestionDeadline(questionIdForMostRecentQuestion).QuestionNumber;
                    DateTime mostRecentQuestionDeadlinePassed = probeGame.GetProbeGameQuestionDeadline(questionIdForMostRecentQuestion).QuestionDeadlineDT;
                    string mostRecentQuestionNameDeadlinePassed = db.Question.Find(questionIdForMostRecentQuestion).Name;

                    int nextQuestionNbr = (mostRecentQuestionNbrPassed + 1 <= probeGame.ProbeGameQuestionDeadlines.Count) ? mostRecentQuestionNbrPassed + 1 : mostRecentQuestionNbrPassed;
                    ProbeGameQuestionDeadline nextpgqd =
                        probeGame.ProbeGameQuestionDeadlines.Where(pgqd => pgqd.QuestionNumber == nextQuestionNbr).FirstOrDefault();
                    DateTime nextQuestionDeadline = nextpgqd.QuestionDeadlineDT;
                    string nextQuestionName = db.Question.Find(nextpgqd.QuestionId).Name;

                    int gameNbrQuestions = probeGame.Game.GameQuestions.Count();
                    mostRecentQuestionNbrPassed--; //calibrate to element 0 base. it was at element 1 base
                    nextQuestionNbr--; //calibrate to element 0 base. it was at element 1 base

                    //Will return -1 if there are no questions submitted yet
                    int maxQuestionNbrSubmitted = probeGame.GetMaxQuestionNbrSubmitted();

                    GameStatusType gameStatusType = GameStatusType.UNKNOWN;
                    if (ProbeValidate.IsGameStartPassed(game) && (mostRecentQuestionNbrPassed < probeGame.ProbeGameQuestionDeadlines.Count - 1))
                    {
                        gameStatusType = GameStatusType.ACTIVE;
                    }
                    else
                    {
                        gameStatusType = GameStatusType.COMPLETED;
                    }

                    GameLMSSummaryReturn gsdr = new GameLMSSummaryReturn
                    {
                        GameStatus = gameStatusType,
                        GameNbrQuestions = gameNbrQuestions,
                        NbrPlayers = probeGame.NbrPlayers,
                        NbrPlayersActive = probeGame.NbrPlayersActive,
                        NbrPlayersInactive = probeGame.NbrPlayers - probeGame.NbrPlayersActive,
                        MostRecentQuestionNbrDeadlinePassed = mostRecentQuestionNbrPassed,
                        MostRecentQuestionNameDeadlinePassed = mostRecentQuestionNameDeadlinePassed,
                        MostRecentQuestionDeadlinePassed = mostRecentQuestionDeadlinePassed,
                        NextQuestionNbr = nextQuestionNbr,
                        NextQuestionName = nextQuestionName,
                        NextQuestionDeadline = nextQuestionDeadline,
                        MaxQuestionNbrSubmitted = maxQuestionNbrSubmitted
                    };

                    return gsdr;

                }
                else
                {
                    //IF WE ARE HERE THAT MEANS THAT THE FIRST QUESTION DEADLINE OF THE GAME HAS NOT PASSED.

                    //We send back a reduced set of information when the game is not active and it hasn't even started
                    int gameNbrQuestions = probeGame.Game.GameQuestions.Count();
                    GameStatusType gameStatusType = GameStatusType.UNKNOWN;
                    int mostRecentQuestionNbrPassed = ProbeConstants.ValueIntNone;
                    string mostRecentQuestionNameDeadlinePassed = ProbeConstants.ValueStringNone;
                    DateTime mostRecentQuestionDeadlinePassed = DateTime.MinValue;
                    int nextQuestionNbr = ProbeConstants.ValueIntNone;
                    string nextQuestionName = ProbeConstants.ValueStringNone;
                    DateTime nextQuestionDeadline = DateTime.MinValue;

                    if (ProbeValidate.IsGameStartPassed(game))
                    {
                        //no most recent question deadline passed. However there is a next question deadline (first question
                        //of the game
                        gameStatusType = GameStatusType.STARTEDNOQUESTIONPASSED;
                        nextQuestionNbr = 0;
                        nextQuestionDeadline = probeGame.ProbeGameQuestionDeadlines[0].QuestionDeadlineDT;
                        nextQuestionName = db.Question.Find(probeGame.ProbeGameQuestionDeadlines[0].QuestionId).Name;
                    }
                    else
                    {
                        gameStatusType = GameStatusType.NOTSTARTED;
                    }

                    int maxQuestionNbrSubmitted = probeGame.GetMaxQuestionNbrSubmitted();

                    GameLMSSummaryReturn gsdr = new GameLMSSummaryReturn
                    {
                        //We will provide information about any players that have submitted already even if the first question's deadline hasn't been reached.
                        GameStatus = gameStatusType,
                        GameNbrQuestions = gameNbrQuestions,
                        NbrPlayers = probeGame.NbrPlayers,
                        NbrPlayersActive = probeGame.NbrPlayersActive,
                        NbrPlayersInactive = probeGame.NbrPlayers - probeGame.NbrPlayersActive,
                        MostRecentQuestionNbrDeadlinePassed = mostRecentQuestionNbrPassed,
                        MostRecentQuestionNameDeadlinePassed = mostRecentQuestionNameDeadlinePassed,
                        MostRecentQuestionDeadlinePassed = mostRecentQuestionDeadlinePassed,
                        NextQuestionNbr = nextQuestionNbr,
                        NextQuestionName = nextQuestionName,
                        NextQuestionDeadline = nextQuestionDeadline,
                        MaxQuestionNbrSubmitted = maxQuestionNbrSubmitted
                    };

                    return gsdr;

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }//public GameLMSSummaryReturn

        [Route("api/Reports/GetPlayerLMSSummaryData/{gameid}/{code}/{playerstatusfilter}")]
        public List<PlayerLMSSummaryReturn> GetPlayerLMSSummaryData(long gameid, string code, int playerstatusfilter)
        {
            /*
            the gameid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameid, code);

            try
            {
                //update player statuses for a game.
                ProbeGame.UpdateAllGamePlayerStatuses();

                DateTime dateTimeNowUTC = DateTime.UtcNow;

                var db = new ProbeDataContext();
                Game game = db.Game.Find(gameid);
                ProbeGame probeGame = new ProbeGame(game);

                //Get information about question that most recently passed the deadline and the next question after that
                long questionIdForMostRecentQuestion = probeGame.GetMostRecentQuestionPassedDeadline(dateTimeNowUTC);

                int mostRecentQuestionNbrPassed = ProbeConstants.ValueIntNone;

                DateTime mostRecentQuestionDeadlinePassed = DateTime.MinValue;
                if (questionIdForMostRecentQuestion != ProbeConstants.NoPrimaryKeyId)
                {
                    mostRecentQuestionNbrPassed = probeGame.GetProbeGameQuestionDeadline(questionIdForMostRecentQuestion).QuestionNumber;
                    mostRecentQuestionDeadlinePassed = probeGame.GetProbeGameQuestionDeadline(questionIdForMostRecentQuestion).QuestionDeadlineDT;
                }

                //NOTE: playerstatusfilter = 0 is ALL players; = 1 ACTIVE players; = 2 INACTIVE players
                var result = db.Database.SqlQuery<PlayerLMSSummaryData>
                                                 ("exec GetPlayerLMSSummary " + gameid + ',' + playerstatusfilter).ToList();

                List<PlayerLMSSummaryReturn> reportData = new List<PlayerLMSSummaryReturn>();
                foreach (PlayerLMSSummaryData row in result)
                {
                    PlayerLMSSummaryReturn plsr = new PlayerLMSSummaryReturn
                    {
                        PlayerName = row.PlayerName,
                        PlayerId = row.PlayerId,
                        PlayerStatus = row.PlayerStatus,
                        PlayerGameReason = row.PlayerGameReason,
                        QuestionNbrLastSubmitted = row.QuestionNbrLastSubmitted - 1, //convert to base 0 element
                        MostRecentQuestionNbrDeadlinePassed = mostRecentQuestionNbrPassed,
                        MostRecentQuestionDeadlinePassed = mostRecentQuestionDeadlinePassed
                    };

                    reportData.Add(plsr);
                }

                return reportData;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }//public PlayerLMSSummaryReturn GetPlayerLMSSummaryData(long gameid, string code)

        [Route("api/Reports/GetPlayerLMSDetailData/{gameid}/{code}/{playerid}")]
        public List<PlayerLMSDetailReturn> GetPlayerLMSSummaryData(long gameid, string code, long playerid)
        {
            /*
            the gameid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameid, code);

            try
            {
                //update player statuses for a game.
                ProbeGame.UpdateAllGamePlayerStatuses();

                //NOTE: playerstatusfilter = 0 is ALL players; = 1 ACTIVE players; = 2 INACTIVE players
                var result = db.Database.SqlQuery<PlayerLMSDetailData>
                                                 ("exec GetPlayerLMSDetail " + gameid + ',' + playerid).ToList();

                bool firstNullSelection = true;
                List<PlayerLMSDetailReturn> reportData = new List<PlayerLMSDetailReturn>();
                foreach (PlayerLMSDetailData row in result)
                {

                    //Will serialize all selections that ARE NOT null OR (the first selection that is NULL and the PlayerGameReason is DEADLINE). That would be the question
                    //that the player just missed the deadline on
                    if (row.SelectedChoices != null ||
                        !row.PlayerStatus && row.SelectedChoices == null && row.PlayerGameReason == Player.PlayerGameReasonType.ANSWER_REASON_DEADLINE && firstNullSelection)
                    {
                        
                        PlayerLMSDetailReturn pldr = new PlayerLMSDetailReturn
                        {
                            PlayerName = row.PlayerName,
                            PlayerId = row.PlayerId,
                            PlayerStatus = row.PlayerStatus,
                            PlayerGameReason = row.PlayerGameReason,
                            QuestionId = row.QuestionId,
                            Question = row.Question,
                            OrderNbr = row.OrderNbr,
                            SelectedChoices = row.SelectedChoices,
                            CorrectChoices = row.CorrectChoices
                        };

                        reportData.Add(pldr);

                        if (row.SelectedChoices == null)
                        {
                            firstNullSelection = false;
                        }

                    }//if (row.SelectedChoices != null)
                }

                return reportData;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }//public PlayerLMSSummaryReturn GetPlayerLMSSummaryData(long gameid, string code)
    
    }
}