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
using Probe.Helpers.Validations; //MNS

namespace Probe.Controllers
{
    public class GameTypesController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: GameTypes
        public ActionResult Index()
        {
            return View(db.GameType.ToList());
        }

        // GET: GameTypes/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GameType gameType = db.GameType.Find(id);
            if (gameType == null)
            {
                return HttpNotFound();
            }
            return View(gameType);
        }

        // GET: GameTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GameTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] GameType gameType)
        {
            if (ModelState.IsValid)
            {
                db.GameType.Add(gameType);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index");
            }

            return View(gameType);
        }

        // GET: GameTypes/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GameType gameType = db.GameType.Find(id);
            if (gameType == null)
            {
                return HttpNotFound();
            }
            return View(gameType);
        }

        // POST: GameTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] GameType gameType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gameType).State = EntityState.Modified;
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index");
            }
            return View(gameType);
        }

        // GET: GameTypes/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GameType gameType = db.GameType.Find(id);
            if (gameType == null)
            {
                return HttpNotFound();
            }
            return View(gameType);
        }

        // POST: GameTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            GameType gameType = db.GameType.Find(id);
            db.GameType.Remove(gameType);
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
    }
}
