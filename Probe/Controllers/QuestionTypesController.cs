﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Probe.DAL;
using ProbeDAL.Models;
using Probe.Helpers.Mics;

namespace Probe.Controllers
{
    public class QuestionTypesController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: QuestionTypes
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            try {
            return View(db.QuestionType.ToList());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View();
            }
        }

        // GET: QuestionTypes/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                QuestionType questionType = db.QuestionType.Find(id);
                if (questionType == null)
                {
                    return HttpNotFound();
                }
                return View(questionType);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View();
            }
        }

        // GET: QuestionTypes/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuestionTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] QuestionType questionType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.QuestionType.Add(questionType);
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                    return RedirectToAction("Index");
                }

                return View(questionType);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View();
            }
        }

        // GET: QuestionTypes/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                QuestionType questionType = db.QuestionType.Find(id);
                if (questionType == null)
                {
                    return HttpNotFound();
                }
                return View(questionType);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View();
            }
        }

        // POST: QuestionTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] QuestionType questionType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(questionType).State = EntityState.Modified;
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                    return RedirectToAction("Index");
                }
                return View(questionType);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View();
            }
        }

        // GET: QuestionTypes/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(long? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                QuestionType questionType = db.QuestionType.Find(id);
                if (questionType == null)
                {
                    return HttpNotFound();
                }
                return View(questionType);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError(ProbeConstants.MSG_UnsuccessfulOperation.ToString(), ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return View();
            }
        }

        // POST: QuestionTypes/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                QuestionType questionType = db.QuestionType.Find(id);
                db.QuestionType.Remove(questionType);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
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
