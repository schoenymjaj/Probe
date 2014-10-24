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

            Session["CurrentSelectedGame"] = SelectedGame;
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"];
            ViewBag.DctGameActive = ProbeValidate.GetAllGamesActiveStatus();

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
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameQuestion.GameId);

            if (ModelState.IsValid)
            {
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
            GameQuestion gameQuestion = db.GameQuestion.Find(id);
            if (gameQuestion == null)
            {
                return HttpNotFound();
            }
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameQuestion.GameId);
            ViewBag.QuestionId = new SelectList(GetRemainingQuestions(gameQuestion.GameId, loggedInUserId,id), "Id", "Name", gameQuestion.QuestionId);
            ViewBag.Weight = new SelectList(weights, DEFAULT_WEIGHT);

            return View(gameQuestion);
        }

        // POST: GameQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GameId,QuestionId,Weight,OrderNbr")] GameQuestion gameQuestion)
        {
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            if (ModelState.IsValid)
            {
                db.Entry(gameQuestion).State = EntityState.Modified;
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
            GameQuestion gameQuestion = db.GameQuestion.Find(id);

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameQuestion.GameId); //persist the selected game

            db.GameQuestion.Remove(gameQuestion);
            db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
            return RedirectToAction("Index", new { SelectedGame = ViewBag.GameId.SelectedValue });
        }

        private IList<Question> GetRemainingQuestions(long SelectedGame, string loggedInUserId, long? gameQuestionId)
        {

            IQueryable<Probe.Models.Question> allQuestions = Enumerable.Empty<Question>().AsQueryable();
            switch (db.Game.Find(SelectedGame).GameType.Name)
            {
                case "Test":
                    allQuestions = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId && cq.Choices.Any(c => c.Correct));
                    break;
                case "Match":
                    allQuestions = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId);
                    break;
                default:
                    allQuestions = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId);
                    break;
                
            }
            var usedQuestions = db.GameQuestion.Where(gq => gq.GameId == SelectedGame).Select(gq => gq.QuestionId);

            if (gameQuestionId != null)
            {
                //include the question for the current GameQuestion
                long currentQuestionId = db.GameQuestion.Find(gameQuestionId).Question.Id;
                return allQuestions.Where(aq => (aq.Id == currentQuestionId) || (!usedQuestions.Contains(aq.Id))).ToList();
            }
            else
            {
                return allQuestions.Where(aq => !usedQuestions.Contains(aq.Id)).ToList();
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
