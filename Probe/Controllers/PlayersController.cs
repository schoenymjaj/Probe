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
using Probe.Helpers.Mics;
using Probe.Helpers.GameHelpers;

namespace Probe.Controllers
{
    public class PlayersController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();
         
        // GET: Players
        public ActionResult Index(int? SelectedGame)
        {
            ViewBag.DctAllGamesActiveStatus = ProbeValidate.GetAllGamesStatus();

            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");
            var games = db.Game
                .Where(g => g.AspNetUsersId == loggedInUserId)
                .OrderBy(g => g.Name);

            if (games.Count() > 0 && !SelectedGame.HasValue)
            {
                SelectedGame = (int)games.First().Id;
            }

            ViewBag.SelectedGame = new SelectList(games, "Id", "Name", SelectedGame);

            return View(db.Player.Where(p => p.GameId == SelectedGame || !SelectedGame.HasValue).ToList());
        }

        // GET: Players/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Player.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // GET: Players/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LastName,FirstName,MiddleName,EmailAddr,MobileNbr,Active")] Player player)
        {
            if (ModelState.IsValid)
            {
                db.Player.Add(player);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index");
            }

            return View(player);
        }

        // GET: Players/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Player.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }

            ViewBag.Sex = EnumHelper.SelectListFor<Person.SexType>(player.Sex);

            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GameId,LastName,FirstName,MiddleName,EmailAddr,MobileNbr,NickName,Sex,Active,SubmitDate,SubmitTime")] Player player)
        {
            if (ModelState.IsValid)
            {
                long gameId = player.GameId;

                db.Entry(player).State = EntityState.Modified;
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index", new { SelectedGame = gameId });
            }
            return View(player);
        }

        // GET: Players/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Player.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            //will delete the game play submissions of the player and then the player.
            Player player = db.Player.Find(id);

            long gameId = player.GameId;

            ProbeGame.DeletePlayer(db, player);

            return RedirectToAction("Index", new { SelectedGame = gameId});
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
