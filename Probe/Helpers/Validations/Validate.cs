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

using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;

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
            status = db.GamePlay.Where(gp => gp.Code == code).Count() > 0;

            return status;
        }

        public static bool IsCodeExistInProbe(long gamePlayId, string code)
        {
            bool status = false;

            var db = new ProbeDataContext();
            status = db.GamePlay.Where(gp => gp.Code == code && gp.Id != gamePlayId).Count() > 0;

            return status;
        }


        public static bool IsGamePlayNameExistForLoggedInUser(string gamePlayName)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.GamePlay.Where(gp => gp.Name == gamePlayName && gp.Game.AspNetUsersId == AspNetUsersId).Count() > 0;

            return status;
        }

        public static bool IsGamePlayNameExistForLoggedInUser(long gamePlayId, string gamePlayName)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.GamePlay.Where(gp => gp.Name == gamePlayName && gp.Game.AspNetUsersId == AspNetUsersId && gp.Id != gamePlayId).Count() > 0;

            return status;
        }

        public static bool IsGamePlayActive(GamePlay gp)
        {
            bool status = false;
            if (((DateTime.Compare(DateTime.Now, gp.StartDate) > 0 &&
                                DateTime.Compare(DateTime.Now, gp.EndDate) <= 0)
                                || (gp.Players.Count() > 0))
                                && !gp.SuspendMode)
            {
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }

        //player name. is it unique. does it meet requirements (returns exceptions if player name invalid
        public static void IsGamePlayPlayerValid(long gamePlayId, Player player)
        {

            //determine if there is another player with the same name that has already submitted for a game play
            var db = new ProbeDataContext();
            int recordCount = db.Player.Where(p => p.GamePlayId == gamePlayId 
                                              && p.FirstName == player.FirstName 
                                              && p.NickName == player.NickName).Count();
            if (recordCount > 0)
            {
                throw new GamePlayDuplicatePlayerNameException();
            }

            if (player.FirstName.Length == 0 || player.FirstName.Length > 10)
            {
                throw new GamePlayInvalidFirstNameException();
            }

            if (player.FirstName.Length == 0 || player.NickName.Length > 10)
            {
                throw new GamePlayInvalidNickNameException();
            }

        }

        public static bool DoesGamePlayHaveSubmissions(long gamePlayId)
        {
            bool status = false;
            var db = new ProbeDataContext();
            int recordCount = db.Player.Where(p => p.GamePlayId == gamePlayId).Count();
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

        #endregion

        #region GameQuestions Validations

        public static bool IsGameQuestionExist(long gameId, long questionId)
        {
            bool status = false;

            var db = new ProbeDataContext();
            status = db.GameQuestion.Where(gq => gq.GameId == gameId && gq.QuestionId == questionId).Count() > 0;

            return status;
        }

        #endregion

        #region Question Validations

        public static bool IsQuestionUsedByActivatedGamePlay(Question question)
        {

            bool status = false;

            ProbeDataContext db = new ProbeDataContext();
            var nbrGamePlaysActiveForQuestion = db.GamePlay
                .Where(gp =>
                ((DateTime.Compare(DateTime.Now, gp.StartDate) > 0 &&
                DateTime.Compare(DateTime.Now, gp.EndDate) <= 0)
                || (gp.Players.Count() > 0))
                && !gp.SuspendMode)
                .Where(gp => gp.Game.GameQuestions
                                .Any(gq => gq.QuestionId == question.Id))
                .Count(); //STILL A COUNT OF GAMEPLAYS

            if (nbrGamePlaysActiveForQuestion > 0) status = true;
            return status;
        }

        public static bool IsQuestionNameExistForLoggedInUser(string questionName)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.Question.Where(q => q.Name == questionName && q.AspNetUsersId == AspNetUsersId).Count() > 0;

            return status;
        }

        public static bool IsQuestionNameExistForLoggedInUser(long questionId, string questionName)
        {
            bool status = false;

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            var db = new ProbeDataContext();
            status = db.Question.Where(q => q.Name == questionName && q.AspNetUsersId == AspNetUsersId && q.Id != questionId).Count() > 0;

            return status;
        }

        public static bool IsQuestionPossessCorrectChoice(long questionId)
        {
            ProbeDataContext db = new ProbeDataContext();
            return db.ChoiceQuestion.Find(questionId).Choices.Count() > 0;

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

        public static bool IsGameUsedByActivatedGamePlay(Game game)
        {

            bool status = false;

            ProbeDataContext db = new ProbeDataContext();
            var nbrGamePlaysActiveForGame = db.GamePlay
                .Where(gp =>
                ((DateTime.Compare(DateTime.Now, gp.StartDate) > 0 &&
                DateTime.Compare(DateTime.Now, gp.EndDate) <= 0)
                || (gp.Players.Count() > 0))
                && !gp.SuspendMode)
                .Where(gp => gp.Game.Id == game.Id)
                .Count(); //STILL A COUNT OF GAMEPLAYS

            if (nbrGamePlaysActiveForGame > 0) status = true;
            return status;
        }

        public static Dictionary<long,bool> GetAllGamePlaysStatus()
        {

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            //All GamePlays Active or Inactive (GamePlayId, true or false)
            ProbeDataContext db = new ProbeDataContext();

            return    db.GamePlay
                        .Where(gp => gp.Game.AspNetUsersId == AspNetUsersId)
                        .Select(gp => new
                        {
                            Id = gp.Id,
                            Active =
                                (((DateTime.Compare(DateTime.Now, gp.StartDate) > 0 &&
                                DateTime.Compare(DateTime.Now, gp.EndDate) <= 0)
                                || (gp.Players.Count() > 0))
                                && !gp.SuspendMode)
                        }).ToDictionary(gp => gp.Id, gp => gp.Active);

        }//public static Dictionary<long,bool> GetGamePlaysStatus()

        public static Dictionary<long, bool> GetAllGamesActiveStatus()
        {

            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            //All Games (Active or InActive)
            ProbeDataContext db = new ProbeDataContext();

            //All Games - Active or Inactive (GameId, true or false)
            return    db.Game
                      .Where(g => g.AspNetUsersId == AspNetUsersId)
                      .Select(g => new
                      {
                          Id = g.Id,
                          Active = g.GamePlays
                        .Where(gp =>
                                ((DateTime.Compare(DateTime.Now, gp.StartDate) > 0 &&
                                DateTime.Compare(DateTime.Now, gp.EndDate) <= 0)
                                || (gp.Players.Count() > 0))
                                && !gp.SuspendMode)
                                .Count() > 0
                      }).ToDictionary(gpa => gpa.Id, gpa => gpa.Active);

        }//public static Dictionary<long, bool> GetGamesStatus()

        public static Dictionary<long, bool> GetAllQuestionsActiveStatus()
        {
            string AspNetUsersId = System.Web.HttpContext.Current.User.Identity.GetUserId();

            //All Questions (Active or InActive). Gets statuses only for the user specified
            ProbeDataContext db = new ProbeDataContext();

            /*
            * Business Rules - Question will be Inactive if 
            *  1. If there are no Game's using the Question
            *                  -OR-
            *  2. If there are no GamePlay's using the Question
            *                  -OR-
            *  3. If there are no GamePlay's using the Question with Player submissions 
            *     -OR- the GamePlay is in suspend mode 
            *     -OR- the date/time is not within the GamePlay Start and End DateTime Window
            *     -OR- a players has NOT submitted a game for the GamePlay
            */

            /*left joined GameQuestion to Question
             * Then got all the games - game plays for each question and determined if the game plays were active or inactive
             * Then (since there may be multiple games with a question, we had to group all the results (quesionId, Active), and 
             *      determine if any of the game-gameplays were Active for any question. If so, then the question was Active
             */
            return db.Question.OfType<ChoiceQuestion>()
                    .Where(q => q.AspNetUsersId == AspNetUsersId)
                    .SelectMany(cq => cq.GameQuestions.DefaultIfEmpty(),
                    (cq, gq) =>
                        new
                        {
                            QuestionId = cq.Id,
                            Active = gq.Game.GamePlays
                            .Where(gp =>
                                        (((DateTime.Compare(DateTime.Now, gp.StartDate) > 0 &&
                                        DateTime.Compare(DateTime.Now, gp.EndDate) <= 0)
                                        || (gp.Players.Count() > 0))
                                        && !gp.SuspendMode)
                                ).Count() > 0
                        }
                    )
                    .GroupBy(qDict => qDict.QuestionId)
                    .Select(qDict => new { QuestionId = qDict.Key, Active = qDict.Any(qAny => qAny.Active) })
                    .ToDictionary(qgp => qgp.QuestionId, qgp => qgp.Active);

        }// public static Dictionary<long, bool> GetQuestionsStatus()


        #endregion


    }

}