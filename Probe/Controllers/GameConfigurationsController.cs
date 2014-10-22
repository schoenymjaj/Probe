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

namespace Probe.Controllers
{
    public class GameConfigurationsController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? SelectedGame)
        {
            //limit the games to only what the user possesses
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            var games = db.Game.Where(g => g.AspNetUsersId == loggedInUserId).OrderBy(g => g.Name).ToList();
            ViewBag.SelectedGame = new SelectList(games, "Id", "Name", SelectedGame);
            int gameId = SelectedGame.GetValueOrDefault();

            IQueryable<GameConfiguration> gameConfiguration = db.GameConfiguration
                                    .Where(gc => (!SelectedGame.HasValue && loggedInUserId != "-1") || gc.GameId == gameId)
                                    .OrderBy(gc => gc.Name)
                                    .Include(g => g.Game);
            return View(gameConfiguration.ToList());
        }

        // GET: GameConfigurations/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GameConfiguration gameConfiguration = db.GameConfiguration.Find(id);
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId); //persist the selected game

            if (gameConfiguration == null)
            {
                return HttpNotFound();
            }
            return View(gameConfiguration);
        }

        // GET: GameConfigurations/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? SelectedGame)
        {
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", SelectedGame);
            return View();
        }

        // POST: GameConfigurations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GameId,Name,Description,Value")] GameConfiguration gameConfiguration)
        {
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId);

            if (ModelState.IsValid)
            {
                db.GameConfiguration.Add(gameConfiguration);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index", new { SelectedGame = ViewBag.GameId.SelectedValue });
            }

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId);
            return View(gameConfiguration);
        }

        // GET: GameConfigurations/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GameConfiguration gameConfiguration = db.GameConfiguration.Find(id);
            if (gameConfiguration == null)
            {
                return HttpNotFound();
            }
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId);
            return View(gameConfiguration);
        }

        // POST: GameConfigurations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GameId,Name,Description,Value")] GameConfiguration gameConfiguration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gameConfiguration).State = EntityState.Modified;
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId);
                return RedirectToAction("Index", new { SelectedGame = ViewBag.GameId.SelectedValue });
            }
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId);
            return View(gameConfiguration);
        }

        // GET: GameConfigurations/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GameConfiguration gameConfiguration = db.GameConfiguration.Find(id);
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId); //persist the selected game

            if (gameConfiguration == null)
            {
                return HttpNotFound();
            }
            return View(gameConfiguration);
        }

        // POST: GameConfigurations/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            GameConfiguration gameConfiguration = db.GameConfiguration.Find(id);

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId); //persist the selected game

            db.GameConfiguration.Remove(gameConfiguration);
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
    }
}
