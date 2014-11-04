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

namespace Probe.Controllers
{
    public class PlayersController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();
         
        // GET: Players
        public ActionResult Index(int? SelectedGamePlay)
        {
            ViewBag.DctGamePlayActive = ProbeValidate.GetAllGamePlaysStatus();

            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");
            var gamePlays = db.GamePlay
                .Where(gp => gp.Game.AspNetUsersId == loggedInUserId)
                .OrderBy(gp => gp.Name);

            if (gamePlays.Count() > 0 && !SelectedGamePlay.HasValue)
            {
                SelectedGamePlay = (int)gamePlays.First().Id;
            }

            ViewBag.SelectedGamePlay = new SelectList(gamePlays, "Id", "Name", SelectedGamePlay);

            return View(db.Player.Where(p => p.GamePlayId == SelectedGamePlay || !SelectedGamePlay.HasValue).ToList());
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
        public ActionResult Create([Bind(Include = "Id,LastName,FirstName,MiddleName,EmailAddr,MobileNbr")] Player player)
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
        public ActionResult Edit([Bind(Include = "Id,GamePlayId,LastName,FirstName,MiddleName,EmailAddr,MobileNbr,NickName,Sex,SubmitDate,SubmitTime")] Player player)
        {
            if (ModelState.IsValid)
            {
                long gamePlayId = player.GamePlayId;

                db.Entry(player).State = EntityState.Modified;
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index", new { SelectedGamePlay = gamePlayId });
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

            long gamePlayId = player.GamePlayId;

            var gpas = db.GamePlayAnswer.Where(gpa => gpa.PlayerId == player.Id);
            foreach (GamePlayAnswer gpa in gpas)
            {
                db.GamePlayAnswer.Remove(gpa);
            }
            db.Player.Remove(player);

            db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
            return RedirectToAction("Index", new { SelectedGamePlay = gamePlayId});
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
