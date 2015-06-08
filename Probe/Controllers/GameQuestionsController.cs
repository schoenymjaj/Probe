using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Probe.DAL;
using Probe.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Probe.Helpers.Validations;
using Probe.Helpers.QuestionHelpers;

namespace Probe.Controllers
{
    public class GameQuestionsController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        private int[] weights = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private const int DEFAULT_WEIGHT = 10;

        // GET: GameQuestions/w Filter
        public ActionResult Index(int? SelectedGame)
        {
            //limit the games to only what the user possesses
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            var games = db.Game.Where(g => g.AspNetUsersId == loggedInUserId).OrderBy(g => g.Name).ToList();

            //if no selected game yet, and there are games owned by current year; we are going to pick the
            //first game that comes up. Doesn't make sense to show ALL games for GameQuestion.
            if (SelectedGame == null && games.Count() > 0) SelectedGame = (int)games.First().Id;
            ViewBag.SelectedGame = new SelectList(games, "Id", "Name", SelectedGame);
            int gameId = SelectedGame.GetValueOrDefault();

            if (SelectedGame != null)
            {
                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsGameForLoggedInUser((long)SelectedGame))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            Session["CurrentSelectedGame"] = SelectedGame;
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"];
            ViewBag.DctGameActive = ProbeValidate.GetAllGamesStatus();

            IQueryable<GameQuestion> gameQuestions = db.GameQuestion
                .Where(gq => gq.GameId == gameId)
                .OrderBy(gq => gq.OrderNbr)
                .Include(g => g.Game);
            return View(gameQuestions.ToList());
        }

        // GET: GameQuestions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameQuestionForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GameQuestion gameQuestion = db.GameQuestion.Find(id);
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameQuestion.GameId); //persist the selected game

            if (gameQuestion == null)
            {
                return HttpNotFound();
            }
            return View(gameQuestion);
        }

        // GET: GameQuestions/Create
        public ActionResult Create(int? SelectedGame)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameForLoggedInUser((long)SelectedGame))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name",SelectedGame);

            ViewBag.QuestionId = new SelectList(GetRemainingQuestions((long)SelectedGame,loggedInUserId, null), "Id", "Name");
            ViewBag.Weight = new SelectList(weights, DEFAULT_WEIGHT);

            GameQuestion gq = new GameQuestion
            {
                OrderNbr = GetNextOrderNbr((long)SelectedGame)
            };

            return View(gq);
        }

        // POST: GameQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GameId,QuestionId,Weight,OrderNbr")] GameQuestion gameQuestion)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameForLoggedInUser((long)gameQuestion.GameId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!ProbeValidate.IsQuestionForLoggedInUser((long)gameQuestion.QuestionId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameQuestion.GameId);

            if (ModelState.IsValid)
            {
                //We need to clone the question and set the UsedInGame field to true
                long clonedQuestionId = ProbeQuestion.CloneQuestion(this, db,true, gameQuestion.QuestionId);
                gameQuestion.QuestionId = clonedQuestionId; //we do a switch to the cloned question

                db.GameQuestion.Add(gameQuestion);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index", new { SelectedGame = ViewBag.GameId.SelectedValue });
            }

            ViewBag.QuestionId = new SelectList(GetRemainingQuestions((long)gameQuestion.GameId, loggedInUserId, gameQuestion.Id), "Id", "Name", gameQuestion.QuestionId);
            return View(gameQuestion);
        }

        // GET: GameQuestions/Edit/5
        public ActionResult Edit(long? id)
        {
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameQuestionForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GameQuestion gameQuestion = db.GameQuestion.Find(id);
            if (gameQuestion == null)
            {
                return HttpNotFound();
            }
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameQuestion.GameId);
            //ViewBag.QuestionId = new SelectList(GetRemainingQuestions(gameQuestion.GameId, loggedInUserId, id), "Id", "Name", gameQuestion.QuestionId);
            var gameQuestionNames = db.GameQuestion.Where(gq => gq.GameId == gameQuestion.GameId).Select(gq => gq.Question);
            ViewBag.QuestionId = new SelectList(gameQuestionNames, "Id", "Name", gameQuestion.QuestionId);
            ViewBag.Weight = new SelectList(weights, DEFAULT_WEIGHT);
            Session["GameQuestion-QuestionId"] = gameQuestion.QuestionId;

            return View(gameQuestion);
        }

        // POST: GameQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GameId,QuestionId,Weight,OrderNbr")] GameQuestion gameQuestion)
        {
            //this question id does not change. For some reason when the dropdown is readonly/disabled it comes back zero. Hence the hack
            gameQuestion.QuestionId = (long)Session["GameQuestion-QuestionId"]; 

            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameQuestionForLoggedInUser(gameQuestion.Id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!ProbeValidate.IsQuestionForLoggedInUser((long)gameQuestion.QuestionId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            if (ModelState.IsValid)
            {
                db.Entry(gameQuestion).State = EntityState.Modified;

                /*
                 * MNS 1/9/15 - No need to give the user the ability to change questions from here. It's easy to delete a question and add another.
                 */

                //if the questionId has changed. then we have to do delete the old UsedInGame question and create the new UsedInGame question
                //if (gameQuestion.QuestionId != (long)Session["GameQuestion-QuestionId"])
                //{
                //    ProbeQuestion.DeleteQuestion(this, db, (long)Session["GameQuestion-QuestionId"]);

                //    //We need to clone the question and set the UsedInGame field to true
                //    long clonedQuestionId = ProbeQuestion.CloneQuestion(this, db, gameQuestion.GameId, gameQuestion.QuestionId);
                //    gameQuestion.QuestionId = clonedQuestionId; //we do a switch to the cloned question
                //}

                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameQuestion.GameId);
                return RedirectToAction("Index", new { SelectedGame = ViewBag.GameId.SelectedValue });
            }
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameQuestion.GameId);
            ViewBag.QuestionId = new SelectList(GetRemainingQuestions(gameQuestion.GameId, loggedInUserId, gameQuestion.Id), "Id", "Name", gameQuestion.QuestionId);
            return View(gameQuestion);
        }

        // GET: GameQuestions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameQuestionForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GameQuestion gameQuestion = db.GameQuestion.Find(id);

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameQuestion.GameId); //persist the selected game

            if (gameQuestion == null)
            {
                return HttpNotFound();
            }
            return View(gameQuestion);
        }

        // POST: GameQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        ////[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameQuestionForLoggedInUser(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GameQuestion gameQuestion = db.GameQuestion.Find(id);

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameQuestion.GameId); //persist the selected game

            db.GameQuestion.Remove(gameQuestion);
            db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

            //Now that the gamequestion record is gone, get rid of the "UsedInGame" question/choice records
            ProbeQuestion.DeleteQuestion(this, db, gameQuestion.QuestionId);

            return RedirectToAction("Index", new { SelectedGame = ViewBag.GameId.SelectedValue });
        }

        //private IList<Question> GetRemainingQuestions(long SelectedGame, string loggedInUserId, long? gameQuestionId)
        //{

        //    //find all questions for the user that are available for use in a game and set in allQuestions var
        //    IQueryable<Probe.Models.Question> allQuestions = Enumerable.Empty<Question>().AsQueryable();
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
            IQueryable<Probe.Models.Question> allQuestions = Enumerable.Empty<Question>().AsQueryable();
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
