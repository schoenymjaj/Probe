using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Probe.DAL;
using Probe.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Probe.Helpers.Validations;

namespace Probe.Controllers
{
    public class GamesController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: Games
        public ActionResult Index()
        {
            //limit the games to only what the user possesses
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");
           
            var game = db.Game.Where(g => g.AspNetUsersId == loggedInUserId).Include(g => g.GameType);
            return View(game.ToList());
        }

        // GET: Games/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Game.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // GET: Games/Create
        public ActionResult Create()
        {
            ViewBag.GameTypeId = new SelectList(db.GameType, "Id", "Name");

            Game game = new Game();
            if (User.Identity.GetUserId() != null)
            {
                game.AspNetUsersId = User.Identity.GetUserId();
            }

            return View(game);
        }

        // POST: Games/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GameTypeId,Name,Description,AspNetUsersId")] Game game)
        {

            ValidateGameCreate(game);
            if (ModelState.IsValid)
            {
                db.Game.Add(game);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index");
            }

            ViewBag.GameTypeId = new SelectList(db.GameType, "Id", "Name", game.GameTypeId);
            return View(game);
        }

        // GET: Games/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Game.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            ViewBag.GameTypeId = new SelectList(db.GameType, "Id", "Name", game.GameTypeId);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GameTypeId,Name,Description,AspNetUsersId")] Game game)
        {
            ValidateGameEdit(game);
            if (ModelState.IsValid)
            {
                db.Entry(game).State = EntityState.Modified;
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index");
            }
            ViewBag.GameTypeId = new SelectList(db.GameType, "Id", "Name", game.GameTypeId);
            return View(game);
        }

        // GET: Games/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Game.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Game game = db.Game.Find(id);
            db.Game.Remove(game);
            db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ValidateGameCreate(Game game)
        {
            //Game Business Rules
            if (ProbeValidate.IsGameNameExistForLoggedInUser(game.Name))
            {
                ModelState.AddModelError("Name", "The game name already exists for the logged in user.");
            }

        }
        private void ValidateGameEdit(Game game)
        {
            //Game Business Rules
            if (ProbeValidate.IsGameNameExistForLoggedInUser(game.Id,game.Name))
            {
                ModelState.AddModelError("Name", "The game name already exists for the logged in user.");
            }

        }
    }
}
