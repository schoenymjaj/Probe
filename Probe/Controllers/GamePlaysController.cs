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
    public class GamePlaysController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: GamePlays
        public ActionResult Index(int? SelectedGame)
        {

            //limit the games to only what the user possesses
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            Session["CurrentSelectedGame"] = SelectedGame;
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"];
            ViewBag.DctGamePlayActive = ProbeValidate.GetAllGamePlaysStatus();

            var games = db.Game.Where(g => g.AspNetUsersId == loggedInUserId).OrderBy(g => g.Name).ToList();
            ViewBag.SelectedGame = new SelectList(games, "Id", "Name", SelectedGame);
            int gameId = SelectedGame.GetValueOrDefault();
                
            IQueryable<GamePlay> gamePlay = db.GamePlay
                .Where(gp => loggedInUserId != "-1" && (gp.Game.AspNetUsersId == loggedInUserId && (gp.GameId == gameId || !SelectedGame.HasValue)))
                .OrderBy(gp => gp.Name)
                .Include(gp => gp.Game);

            return View(gamePlay.ToList());
        }

        // GET: GamePlays/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GamePlay gamePlay = db.GamePlay.Find(id);

            ViewBag.GameId = new SelectList(db.Game , "Id", "Name", gamePlay.GameId); //persist the selected game

            if (gamePlay == null)
            {
                return HttpNotFound();
            }
            return View(gamePlay);
        }

        // GET: GamePlays/Create
        public ActionResult Create(int? SelectedGame)
        {
            string loggedInUserId = User.Identity.GetUserId();
            ViewBag.GameId = new SelectList(db.Game.Where(g => g.AspNetUsersId == loggedInUserId), "Id", "Name", SelectedGame); //persist the selected game
            return View();
        }

        // POST: GamePlays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GameId,Name,Description,Code,GameUrl,StartDate,EndDate,SuspendMode,TestMode,ClientReportAccess")] GamePlay gamePlay)
        {
            string loggedInUserId = User.Identity.GetUserId();
            ViewBag.GameId = new SelectList(db.Game.Where(g => g.AspNetUsersId == loggedInUserId), "Id", "Name", gamePlay.GameId);

            ValidateGamePlayCreate(gamePlay);
            if (ModelState.IsValid)
            {
                db.GamePlay.Add(gamePlay);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index", new { SelectedGame = ViewBag.GameId.SelectedValue });
            }

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gamePlay.GameId);
            return View(gamePlay);
        }

        // GET: GamePlays/Edit/5
        public ActionResult Edit(long? id)
        {
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"];
            ViewBag.DctGamePlayActive = ProbeValidate.GetAllGamePlaysStatus();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GamePlay gamePlay = db.GamePlay.Find(id);
            if (gamePlay == null)
            {
                return HttpNotFound();
            }
            return View(gamePlay);
        }

        // POST: GamePlays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GameId,Name,Description,Code,GameUrl,StartDate,EndDate,SuspendMode,TestMode,ClientReportAccess")] GamePlay gamePlay)
        {
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"];
            ViewBag.DctGamePlayActive = ProbeValidate.GetAllGamePlaysStatus();

            ValidateGamePlayEdit(gamePlay);
            if (ModelState.IsValid)
            {
                db.Entry(gamePlay).State = EntityState.Modified;
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                return RedirectToAction("Index", new { SelectedGame = ViewBag.CurrentSelectedGame });
            }
            return View(gamePlay);
        }

        // GET: GamePlays/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GamePlay gamePlay = db.GamePlay.Find(id);

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gamePlay.GameId); //persist the selected game

            if (gamePlay == null)
            {
                return HttpNotFound();
            }
            return View(gamePlay);
        }

        // POST: GamePlays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            GamePlay gamePlay = db.GamePlay.Find(id);

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gamePlay.GameId); //persist the selected game

            db.GamePlay.Remove(gamePlay);
            db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
            return RedirectToAction("Index", new { SelectedGame = ViewBag.GameId.SelectedValue });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ValidateGamePlayCreate(GamePlay gamePlay)
        {
            //GamePlay Business Rules
            if (ProbeValidate.IsCodeExistInProbe(gamePlay.Code))
            {
                ModelState.AddModelError("Code", "The code already exists in Probe.");
            }
            if (ProbeValidate.IsGamePlayNameExistForLoggedInUser(gamePlay.Name))
            {
                ModelState.AddModelError("Name", "The game play name already exists for the logged in user.");
            }
            if (!ProbeValidate.DoesGameHaveQuestions(gamePlay.GameId))
            {
                ModelState.AddModelError("GameId", "The game for a game play must have at least one question.");
            }

        }
        private void ValidateGamePlayEdit(GamePlay gamePlay)
        {
            //GamePlay Business Rules
            if (ProbeValidate.IsCodeExistInProbe(gamePlay.Id,gamePlay.Code))
            {
                ModelState.AddModelError("Code", "The code already exists in Probe.");
            }
            if (ProbeValidate.IsGamePlayNameExistForLoggedInUser(gamePlay.Id,gamePlay.Name))
            {
                ModelState.AddModelError("Name", "The game play name already exists for the logged in user.");
            }
            if (!ProbeValidate.DoesGameHaveQuestions(gamePlay.GameId))
            {
                ModelState.AddModelError("GameId", "The game for a game play must have at least one question.");
            }


        }
    }
}
