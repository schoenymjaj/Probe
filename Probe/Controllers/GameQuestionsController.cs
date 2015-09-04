using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Probe.DAL;
using ProbeDAL.Models;
using Probe.Models.View;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Probe.Helpers.Validations;
using Probe.Helpers.QuestionHelpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Probe.Controllers
{
    public class GameQuestionsController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        private int[] weights = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private const int DEFAULT_WEIGHT = 10;

        // GET: Drag and Drop Questions to a Game 
        public ActionResult GameQuestions(long gameid)
        {
            Game game = db.Game.Find(gameid);

            ViewBag.GameId = gameid;
            ViewBag.GameName = game.Name;
            ViewBag.GameEditable = !ProbeValidate.DoesGameHaveSubmissions(gameid) &&
                                   !ProbeValidate.IsGameActive(game);
            return View();
        }

        /*
         * Get gamequestions for a specific game (support GameQuestions Kendo)
         */
        public JsonResult GetGameQuestions([DataSourceRequest]DataSourceRequest request, long gameid)
        {
            try
            {
                var gameQuestionDTOs = db.GameQuestion.Where(gq => gq.GameId == gameid)
                    .Select(gq => new GameQuestionDTO
                    {
                        Id = gq.Id,
                        GameId = gq.GameId,
                        QuestionId = gq.QuestionId,
                        QuestionTypeId = gq.Question.QuestionTypeId,
                        OrderNbr = gq.OrderNbr,
                        Weight = gq.Weight,
                        Name = gq.Question.Name,
                        Text = gq.Question.Text,
                        TestEnabled = db.ChoiceQuestion.Where(cq => cq.Id == gq.QuestionId).FirstOrDefault().Choices.Any(c => c.Correct),
                        ChoicesCount = db.ChoiceQuestion.Where(cq => cq.Id == gq.QuestionId).FirstOrDefault().Choices.Count()
                    }).OrderBy(gq => gq.OrderNbr);


                return this.Json(gameQuestionDTOs.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }//public JsonResult GetGameQuestions([DataSourceRequest]DataSourceRequest request)

        /*
         * Update gamequestion (support GameQuestions Kendo)
         */
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest dsRequest, GameQuestionDTO gameQuestionDTO)
        {

            try
            {
                //we are only updating the Game Question order number and weight
                GameQuestion gameQuestion = db.GameQuestion.Find(gameQuestionDTO.Id);
                gameQuestion.OrderNbr = gameQuestionDTO.OrderNbr;
                gameQuestion.Weight = gameQuestionDTO.Weight;

                db.Entry(gameQuestion).State = EntityState.Modified;
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                return Json(new[] { gameQuestionDTO }.ToDataSourceResult(dsRequest, ModelState));

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }
        }

        /*
         * Create gamequestion (support GameQuestions Kendo)
         */
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create([DataSourceRequest] DataSourceRequest request, GameQuestionDTO gameQuestionDTO)
        {

            try
            {

                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsGameForLoggedInUser((long)gameQuestionDTO.GameId))
                {
                    ModelState.AddModelError("", "The attempt to add a game question was not successful");
                    return Json(ModelState.IsValid ? true : ModelState.ToDataSourceResult());
                }
                if (!ProbeValidate.IsQuestionForLoggedInUser((long)gameQuestionDTO.QuestionId))
                {
                    ModelState.AddModelError("", "The attempt to add a game question was not successful");
                    return Json(ModelState.IsValid ? true : ModelState.ToDataSourceResult());
                }

                //transform DTO to business object (GameQuestion)
                GameQuestion gameQuestion = new GameQuestion
                {
                    Id = 0,
                    GameId = gameQuestionDTO.GameId,
                    QuestionId = gameQuestionDTO.QuestionId,
                    OrderNbr = gameQuestionDTO.OrderNbr,
                    Weight = gameQuestionDTO.Weight
                };

                //We need to clone the question and set the UsedInGame field to true. The cloned questions
                //is what will be associated with the game
                Dictionary<long,long> choiceXreference = new Dictionary<long,long>();
                long clonedQuestionId = ProbeQuestion.CloneQuestion(this, db, true, gameQuestion.QuestionId, ref choiceXreference);
                gameQuestion.QuestionId = clonedQuestionId; //we do a switch to the cloned question


                db.GameQuestion.Add(gameQuestion);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                gameQuestionDTO.Id = gameQuestion.Id; //pass back the new Id to the client

                return Json(new[] { gameQuestionDTO }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.IsValid ? true : ModelState.ToDataSourceResult());
            }

        }

        /*
         * Delete gamequestion (support GameQuestions Kendo)
         */
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Delete([DataSourceRequest] DataSourceRequest request, GameQuestionDTO gameQuestionDTO)
        {
            try
            {
                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsGameQuestionForLoggedInUser(gameQuestionDTO.Id))
                {
                    ModelState.AddModelError("", "The attempt to remove a game question was not successful");
                    return Json(ModelState.IsValid ? true : ModelState.ToDataSourceResult());
                }


                GameQuestion gameQuestion = db.GameQuestion.Find(gameQuestionDTO.Id);

                db.GameQuestion.Remove(gameQuestion);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                //Now that the gamequestion record is gone, get rid of the "UsedInGame" question/choice records
                ProbeQuestion.DeleteQuestion(this, db, gameQuestion.QuestionId);


                return Json(new[] { gameQuestionDTO }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }//public JsonResult DeleteGameQuestion([DataSourceRequest] DataSourceRequest request, GameQuestionDTO gameQuestionDTO)

        //private IList<Question> GetRemainingQuestions(long SelectedGame, string loggedInUserId, long? gameQuestionId)
        //{

        //    //find all questions for the user that are available for use in a game and set in allQuestions var
        //    IQueryable<ProbeDAL.Models.Question> allQuestions = Enumerable.Empty<Question>().AsQueryable();
        //    switch (db.Game.Find(SelectedGame).GameType.Name)
        //    {
        //        case "Test":
        //            allQuestions = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId && !cq.UsedInGame && cq.Choices.Any(c => c.Correct));
        //            break;
        //        case "Match":
        //            allQuestions = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId && !cq.UsedInGame);
        //            break;
        //        default:
        //            allQuestions = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId && !cq.UsedInGame);
        //            break;
                
        //    }

        //    //get all questions (id's only) that are currently in use 
        //    var usedQuestions = db.GameQuestion.Where(gq => gq.GameId == SelectedGame).Select(gq => gq.QuestionId); //THIS IS THE ORIGINAL

        //    if (gameQuestionId != null)
        //    {
        //        //include the question for the current GameQuestion
        //        long currentQuestionId = db.GameQuestion.Find(gameQuestionId).Question.Id;
        //        return allQuestions.Where(aq => (aq.Id == currentQuestionId) || (!usedQuestions.Contains(aq.Id))).OrderBy(aq => aq.Name).ToList();
        //    }
        //    else
        //    {
        //        return allQuestions.Where(aq => !usedQuestions.Contains(aq.Id)).OrderBy(aq => aq.Name).ToList();
        //    }

        //}

        private IList<Question> GetRemainingQuestions(long SelectedGame, string loggedInUserId, long? gameQuestionId)
        {

            //find all questions for the user that are available for use in a game and set in allQuestions var
            IQueryable<ProbeDAL.Models.Question> allQuestions = Enumerable.Empty<Question>().AsQueryable();
            switch (db.Game.Find(SelectedGame).GameType.Name)
            {
                case "Test":
                    allQuestions = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId && !cq.UsedInGame && cq.Choices.Any(c => c.Correct));
                    break;
                case "Last Man Standing":
                    allQuestions = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId && !cq.UsedInGame && cq.Choices.Any(c => c.Correct));
                    break;
                case "Match":
                    allQuestions = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId && !cq.UsedInGame);
                    break;
                default:
                    allQuestions = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId && !cq.UsedInGame);
                    break;

            }

            //get all questions (question names only) that are currently in use 
            var usedQuestionNames = db.GameQuestion.Where(gq => gq.GameId == SelectedGame).Select(gq => gq.Question.Name); //THIS IS THE ORIGINAL

            if (gameQuestionId != null)
            {
                //include the question for the current GameQuestion. And remove all questions that possess a name that is already been used
                string currentQuestionName = db.GameQuestion.Find(gameQuestionId).Question.Name;

                return allQuestions.Where(aq => (aq.Name == currentQuestionName) || (!usedQuestionNames.Contains(aq.Name))).OrderBy(aq => aq.Name).ToList();
            }
            else
            {
                //remove all questions that possess a name that is already been used
                return allQuestions.Where(aq => !usedQuestionNames.Contains(aq.Name)).OrderBy(aq => aq.Name).ToList();
            }

        }

        private int GetNextOrderNbr(long SelectedGame) {
            if (db.GameQuestion.Where(gq => gq.GameId == SelectedGame).Count() > 0)
            {
                return (int)db.GameQuestion.Where(gq => gq.GameId == SelectedGame).Max(gq => gq.OrderNbr) + 1;
            }
            else
            {
                return 1;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
