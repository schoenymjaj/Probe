using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using Probe.DAL;
using Probe.Models;
using Probe.Helpers.Exceptions;
using Probe.Helpers.Mics;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace Probe.Helpers.Validations
{
    public static class ProbeValidate
    {

        #region GamePlay Validations

        public static bool IsCodeExistInProbe(string code)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.Game.Where(g => g.Code == code).Count() > 0;

            return status;
        }

        public static bool IsCodeExistInProbe(long gameId, string code)
        {
            bool status = false;

            var db = new ProbeDataContext();
            status = db.Game.Where(g => g.Code == code && g.Id != gameId).Count() > 0;

            return status;
        }

        public static bool IsCodeValid(string code)
        {
            /*
             *  Code should be only letters, numbers, and space. There should
             *  be no leading or trailing spaces either
             */
            bool returnStatus = false;

            if (code == null)
            {
                return false; //code wasn't entered. 
            }

            if (code.TrimStart().TrimEnd() == code)
            {

                if (Regex.Matches(code, "[a-z,A-Z,0-9, ]+").Count == 1)
                {
                    if (Regex.Matches(code, "[a-z,A-Z,0-9, ]+")[0].ToString() == code)
                    {
                        returnStatus = true;
                    }
                }
            }


            return returnStatus;
        }

        //player name. is it unique. does it meet requirements (returns exceptions if player name invalid
        //Validation is a function of first name, nick name, last name, email address
        public static void IsGamePlayerValid(long gameId, Player player)
        {

            //determine if there is another player with the same name that has already submitted for a game play
            var db = new ProbeDataContext();
            int recordCount = db.Player.Where(p => p.GameId == gameId 
                                              && p.FirstName == player.FirstName 
                                              && p.NickName == player.NickName).Count();
            if (recordCount > 0)
            {
                throw new GameDuplicatePlayerNameException();
            }

            if (player.FirstName.Length == 0 || player.FirstName.Length > 10)
            {
                throw new GameInvalidFirstNameException();
            }

            if (player.FirstName.Length == 0 || player.NickName.Length > 10)
            {
                throw new GameInvalidNickNameException();
            }

        }

        public static bool DoesGameHaveSubmissions(long gameId)
        {
            bool status = false;
            var db = new ProbeDataContext();
            int recordCount = db.Player.Where(p => p.GameId == gameId).Count();
            if (recordCount > 0)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;

        }

        public static void ValidateGameCodeVersusId(long gameId, string code)
        {
            var db = new ProbeDataContext();

            var game = db.Game.Where(g => g.Id == gameId && g.Code == code);
            if (game.Count() != 1)
            {
                throw new ApiArgException("The GameId and GameCode do not correlate. GameId: " + gameId + " GameCode: " + code);
            }

        }

        #endregion

        #region GameQuestions Validations

        public static bool IsGameQuestionExist(long gameId, long questionId)
        {
            bool status = false;

            var db = new ProbeDataContext();
            status = db.GameQuestion.Where(gq => gq.GameId == gameId && gq.QuestionId == questionId).Count() > 0;

            return status;
        }

        public static bool IsGameQuestionForLoggedInUser(long gameQuestionId)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.GameQuestion.Where(gq => gq.Id == gameQuestionId && gq.Game.AspNetUsersId == AspNetUsersId).Count() > 0;

            return status;
        }

        #endregion

        #region Question Validations

        public static bool IsQuestionUsedByActivatedGame(Question question)
        {

            bool status = false;

            ProbeDataContext db = new ProbeDataContext();
            var nbrGamesActiveForQuestion = db.Game
                .Where(g =>
                ((DateTime.Compare(DateTime.UtcNow, g.StartDate) > 0 &&
                DateTime.Compare(DateTime.UtcNow, g.EndDate) <= 0)
                || (g.Players.Count() > 0))
                && !g.SuspendMode
                && g.Published)
                .Where(g => g.GameQuestions
                                .Any(gq => gq.QuestionId == question.Id))
                .Count(); //STILL A COUNT OF GAMEPLAYS

            if (nbrGamesActiveForQuestion > 0) status = true;
            return status;
        }

        public static bool IsQuestionNameExistForLoggedInUser(string questionName)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.Question.Where(q => q.Name == questionName && !q.UsedInGame && q.AspNetUsersId == AspNetUsersId).Count() > 0;

            return status;
        }

        public static bool IsQuestionForLoggedInUser(long questionId)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.Question.Where(q => q.Id == questionId && q.AspNetUsersId == AspNetUsersId).Count() > 0;

            return status;
        }

        public static bool IsQuestionNameExistForLoggedInUser(long questionId, string questionName)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            //check if a question name already exists that is not the selected and is not used. We don't look at used in game questions
            var db = new ProbeDataContext();
            status = db.Question.Where(q => q.Name == questionName && !q.UsedInGame && q.AspNetUsersId == AspNetUsersId && q.Id != questionId).Count() > 0;

            return status;
        }

        public static bool IsQuestionPossessCorrectChoice(long questionId)
        {
            ProbeDataContext db = new ProbeDataContext();
            return db.ChoiceQuestion.Find(questionId).Choices.Count(c => c.Correct) > 0;
        }

        public static bool IsQuestionPossessCorrectChoice(long questionId, long choiceId)
        {
            ProbeDataContext db = new ProbeDataContext();
            return db.ChoiceQuestion.Find(questionId).Choices.Count(c => c.Correct && c.Id != choiceId) > 0;
        }

        public static Dictionary<long, bool> GetAllQuestionPossessCorrectChoice()
        {

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            //All Questions - Do they possess a correct choice
            ProbeDataContext db = new ProbeDataContext();

            return db.ChoiceQuestion
                    .Where(cq => cq.AspNetUsersId == AspNetUsersId && !cq.UsedInGame)
                    .Select(cq => new
                    {
                        Id = cq.Id,
                        CanUseForTest = cq.Choices.Count(c => c.Correct) > 0
                    }).ToDictionary(gp => gp.Id, gp => gp.CanUseForTest);

        }//public static Dictionary<long,bool> GetAllQuestionPossessCorrectChoice()

        #endregion

        #region Choice Validations

        public static bool IsChoiceForLoggedInUser(long choiceId)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.Choice.Where(c => c.Id == choiceId && c.ChoiceQuestion.AspNetUsersId == AspNetUsersId).Count() > 0;

            return status;
        }

        #endregion

        #region Game Validations

        public static bool IsGameNameExistForLoggedInUser(string gameName)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.Game.Where(g => g.Name == gameName && g.AspNetUsersId == AspNetUsersId).Count() > 0;

            return status;
        }

        public static bool IsGameNameExistForLoggedInUser(long gameId, string gameName)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.Game.Where(g => g.Name == gameName && g.AspNetUsersId == AspNetUsersId && g.Id != gameId).Count() > 0;

            return status;
        }

        public static bool IsGameForLoggedInUser(long gameId)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.Game.Where(g => g.AspNetUsersId == AspNetUsersId && g.Id == gameId).Count() > 0;

            return status;
        }

        public static bool DoesGameHaveQuestions(long gameId)
        {
            bool status = false;
            var db = new ProbeDataContext();
            int recordCount = db.Game.Find(gameId).GameQuestions.Count();
            if (recordCount > 0)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }

        public static bool IsGameActive(Game g)
        {
            bool status = false;
            if (((DateTime.Compare(DateTime.UtcNow, g.StartDate) > 0 &&
                                DateTime.Compare(DateTime.UtcNow, g.EndDate) <= 0))
                                && !g.SuspendMode
                                && g.Published)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }//public static bool IsGameActive(Game g)

        public static bool IsGameStartPassed(Game g)
        {
            bool status = false;
            if ((DateTime.Compare(DateTime.UtcNow, g.StartDate) > 0))
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }//public static bool IsGameStartPassed(Game g)

        ////DEPRECATED MNS 3/25/15
        //public static bool DoesGameHaveAGamePlay(long gameId)
        //{
        //    bool status = false;
        //    var db = new ProbeDataContext();
        //    int recordCount = db.Game.Find(gameId).GamePlays.Count();
        //    if (recordCount > 0)
        //    {
        //        status = true;
        //    }
        //    else
        //    {
        //        status = false;
        //    }

        //    return status;
        //}

        ////DEPRECATED MNS 3/25/15
        //public static bool IsGameUsedByActivatedGamePlay(Game game)
        //{

        //    bool status = false;

        //    ProbeDataContext db = new ProbeDataContext();
        //    var nbrGamePlaysActiveForGame = db.GamePlay
        //        .Where(gp =>
        //        ((DateTime.Compare(DateTime.Now, gp.StartDate) > 0 &&
        //        DateTime.Compare(DateTime.Now, gp.EndDate) <= 0)
        //        || (gp.Players.Count() > 0))
        //        && !gp.SuspendMode
        //        && g.Published)
        //        .Where(gp => gp.Game.Id == game.Id)
        //        .Count(); //STILL A COUNT OF GAMEPLAYS

        //    if (nbrGamePlaysActiveForGame > 0) status = true;
        //    return status;
        //}

        /*
         * Any game returns active if
         * (Current datetime is in the window of game start date and end date OR
         * Player count is greater than zero) AND game is NOT suspended
         */
        public static Dictionary<long,bool> GetAllGamesStatus()
        {

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            //All GamePlays Active or Inactive (GamePlayId, true or false)
            ProbeDataContext db = new ProbeDataContext();

            return    db.Game
                        .Where(g => g.AspNetUsersId == AspNetUsersId)
                        .Select(g => new
                        {
                            Id = g.Id,
                            Active =
                                (((DateTime.Compare(DateTime.UtcNow, g.StartDate) > 0 &&
                                DateTime.Compare(DateTime.UtcNow, g.EndDate) <= 0)
                                || (g.Players.Count() > 0))
                                && !g.SuspendMode
                                && g.Published)
                        }).ToDictionary(g => g.Id, g => g.Active);

        }//public static Dictionary<long,bool> GetGamesStatus()

        /*
         * Will return a List of Games that are LMS games AND
         * (Current datetime is in the window of game start date and end date AND at least one
         * Player is (Active) AND game is NOT suspended
         * 
         * returns 
         */ 
        public static IList<Game> GetAllActiveLMSGamesWithActivePlayers()
        {
            ProbeDataContext db = new ProbeDataContext();

            return db.Game
                        .Where(g => g.GameType.Name == ProbeConstants.LMSGameType &&
                               (DateTime.Compare(DateTime.UtcNow, g.StartDate) > 0 &&
                                DateTime.Compare(DateTime.UtcNow, g.EndDate) <= 0) &&
                                //(g.Players.Where(p => p.Active && p.GameAnswers.Count() != db.GameQuestion.Where(gq => gq.GameId == g.Id).Count()).Count() > 0) &&
                                (g.Players.Where(p => p.Active).Count() > 0) 
                                && !g.SuspendMode
                                && g.Published).ToList();

        }//public static Dictionary<long,bool> GetAllGamesStatusForLMS()

        //DEPRECATED MNS - 3/25/15
        //public static Dictionary<long, bool> GetAllGamesActiveStatus()
        //{

        //    string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

        //    //All Games (Active or InActive)
        //    ProbeDataContext db = new ProbeDataContext();

        //    //All Games - Active or Inactive (GameId, true or false)
        //    return    db.Game
        //              .Where(g => g.AspNetUsersId == AspNetUsersId)
        //              .Select(g => new
        //              {
        //                  Id = g.Id,
        //                  Active = g
        //                .Where(g =>
        //                        ((DateTime.Compare(DateTime.Now, g.StartDate) > 0 &&
        //                        DateTime.Compare(DateTime.Now, g.EndDate) <= 0)
        //                        || (g.Players.Count() > 0))
        //                        && !g.SuspendMode
        //                        && g.Published)
        //                        .Count() > 0
        //              }).ToDictionary(gpa => gpa.Id, gpa => gpa.Active);

        //}//public static Dictionary<long, bool> GetGamesStatus()

        //public static Dictionary<long, bool> GetAllQuestionsActiveStatus()
        //{
        //    string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

        //    //All Questions (Active or InActive). Gets statuses only for the user specified
        //    ProbeDataContext db = new ProbeDataContext();

        //    /*
        //    * Business Rules - Question will be Inactive if 
        //    *  1. If there are no Game's using the Question
        //    *                  -OR-
        //    *  2. If there are no GamePlay's using the Question
        //    *                  -OR-
        //    *  3. If there are no GamePlay's using the Question with Player submissions 
        //    *     -OR- the GamePlay is in suspend mode 
        //    *     -OR- the date/time is not within the GamePlay Start and End DateTime Window
        //    *     -OR- a players has NOT submitted a game for the GamePlay
        //    */

        //    /*left joined GameQuestion to Question
        //     * Then got all the games - game plays for each question and determined if the game plays were active or inactive
        //     * Then (since there may be multiple games with a question, we had to group all the results (quesionId, Active), and 
        //     *      determine if any of the game-gameplays were Active for any question. If so, then the question was Active
        //     */
        //    return db.Question.OfType<ChoiceQuestion>()
        //            .Where(q => q.AspNetUsersId == AspNetUsersId)
        //            .SelectMany(cq => cq.GameQuestions.DefaultIfEmpty(),
        //            (cq, gq) =>
        //                new
        //                {
        //                    QuestionId = cq.Id,
        //                    Active = gq.Game
        //                    .Where(g =>
        //                                (((DateTime.Compare(DateTime.Now, g.StartDate) > 0 &&
        //                                DateTime.Compare(DateTime.Now, g.EndDate) <= 0)
        //                                || (g.Players.Count() > 0))
        //                                && !g.SuspendMode
        //                                && g.Published)
        //                        ).Count() > 0
        //                }
        //            )
        //            .GroupBy(qDict => qDict.QuestionId)
        //            .Select(qDict => new { QuestionId = qDict.Key, Active = qDict.Any(qAny => qAny.Active) })
        //            .ToDictionary(qgp => qgp.QuestionId, qgp => qgp.Active);

        //}// public static Dictionary<long, bool> GetQuestionsStatus()

        public static Dictionary<long, bool> GetAllGamesDoesHaveQuestions()
        {

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            //All Games
            ProbeDataContext db = new ProbeDataContext();

            //All Games - Active or Inactive (GameId, true or false)
            return db.Game
                      .Where(g => g.AspNetUsersId == AspNetUsersId)
                      .Select(g => new
                      {
                          Id = g.Id,
                          DoesHaveQuestions = g.GameQuestions.Count() > 0
                      }).ToDictionary(gq => gq.Id, gq => gq.DoesHaveQuestions);

        }//public static Dictionary<long, bool> GetAllGamesDoesHaveQuestions()

        //public static Dictionary<long, bool> GetAllGamesHaveGamePlays()
        //{

        //    string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

        //    //All Games
        //    ProbeDataContext db = new ProbeDataContext();

        //    //All Games - Active or Inactive (GameId, true or false)
        //    return db.Game
        //              .Where(g => g.AspNetUsersId == AspNetUsersId)
        //              .Select(g => new
        //              {
        //                  Id = g.Id,
        //                  DoesHaveGamePlays = g.GamePlays.Count() > 0
        //              }).ToDictionary(gp => gp.Id, gp => gp.DoesHaveGamePlays);

        //}//public static Dictionary<long, bool> GetAllGamesHaveGamePlays()

        #endregion

        #region Player Validations

        public static void ValidateGameCodeVersusPlayerId(long playerId, string code)
        {
            var db = new ProbeDataContext();

            bool doesGameCodeRelateWithPlayerId = db.Player.Where(p => p.Id == playerId && p.Game.Code == code).Count() > 0;
            if (!doesGameCodeRelateWithPlayerId)
            {
                throw new ApiArgException("The PlayerId and GameCode do not correlate. PlayerId: " + playerId + " GameCode: " + code);
            }

        }

        public static bool IsPlayerHaveAnyAnswers(long playerId)
        {
            var db = new ProbeDataContext();

            bool returnStatus = false;
            if (db.GameAnswer.Where(ga => ga.PlayerId == playerId).Count() > 0)
            {
                returnStatus = true;
            }

            return returnStatus;

        }

        #endregion


    }

}