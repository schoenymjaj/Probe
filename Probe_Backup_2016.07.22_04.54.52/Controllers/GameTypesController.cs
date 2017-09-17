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
using Probe.Helpers.Validations; //MNS
using Probe.Helpers.Mics;

namespace Probe.Controllers
{
    public class GameTypesController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: GameTypes
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.GameType.ToList());
        }

        // GET: GameTypes/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(long? id)
        {
            try {
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
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View();
            }
        }

        // GET: GameTypes/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: GameTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] GameType gameType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.GameType.Add(gameType);
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                    return RedirectToAction("Index");
                }

                return View(gameType);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View();
            }
        }

        // GET: GameTypes/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(long? id)
        {
            try
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
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View();
            }
        }

        // POST: GameTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] GameType gameType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(gameType).State = EntityState.Modified;
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                    return RedirectToAction("Index");
                }
                return View(gameType);

            }  catch (Exception ex) {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View(gameType);
            }
        }

        // GET: GameTypes/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(long? id)
        {
            try
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
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View();
            }
        }

        // POST: GameTypes/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                GameType gameType = db.GameType.Find(id);
                db.GameType.Remove(gameType);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View();
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
