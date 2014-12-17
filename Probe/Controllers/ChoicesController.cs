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
    public class ChoicesController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        public ActionResult Index(long? SelectedQuestion)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsQuestionForLoggedInUser((long)SelectedQuestion))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //limit the questions to only what the user possesses
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            ViewBag.DctQuestionActive = ProbeValidate.GetAllQuestionsActiveStatus();

            var questions = db.Question.Where(q => q.AspNetUsersId == loggedInUserId).OrderBy(q => q.Name).ToList();
            ViewBag.SelectedQuestion = new SelectList(questions, "Id", "Name", SelectedQuestion);
            long questionId = SelectedQuestion.GetValueOrDefault();

            var choices = db.Choice.Where(c => c.ChoiceQuestionId == questionId);

            return View(choices.ToList());
        }



        // GET: Choices/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsChoiceForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Choice choice = db.Choice.Find(id);
            if (choice == null)
            {
                return HttpNotFound();
            }

            ViewBag.SelectedQuestion = new SelectList(db.Question, "Id", "Name", choice.ChoiceQuestionId);
            return View(choice);
        }

        // GET: Choices/Create
        public ActionResult Create(long? SelectedQuestion)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsQuestionForLoggedInUser((long)SelectedQuestion))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.SelectedQuestion = new SelectList(db.Question, "Id", "Name", SelectedQuestion);
            ChoiceQuestion cq = (ChoiceQuestion)db.Question.Find(SelectedQuestion);

            Choice c = new Choice
            {
                ChoiceQuestionId = (long)SelectedQuestion,
                OrderNbr = GetNextOrderNbr((long)SelectedQuestion),
                ChoiceQuestion = new ChoiceQuestion {
                    Id = (long)SelectedQuestion,
                    Name = cq.Name,
                    Text = cq.Text
                }
            };

            return View(c);
        }

        // POST: Choices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ChoiceQuestionId,Name,Text,Correct,OrderNbr")] Choice choice)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsQuestionForLoggedInUser(choice.ChoiceQuestionId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (choice.Correct) ValidateChoice(choice.ChoiceQuestionId); //only validate if choice selected uses correct
            if (ModelState.IsValid)
            {
                db.Choice.Add(choice);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index", "Choices", new { SelectedQuestion = choice.ChoiceQuestionId });
            }

            ViewBag.SelectedQuestion = new SelectList(db.Question, "Id", "Name", choice.ChoiceQuestionId);
            return View(choice);
        }

        // GET: Choices/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsChoiceForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Choice choice = db.Choice.Find(id);
            if (choice == null)
            {
                return HttpNotFound();
            }
            ViewBag.SelectedQuestion = new SelectList(db.Question, "Id", "Name", choice.ChoiceQuestionId);
            return View(choice);
        }

        // POST: Choices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ChoiceQuestionId,Name,Text,Correct,OrderNbr")] Choice choice)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsChoiceForLoggedInUser(choice.Id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (choice.Correct) ValidateChoice(choice.ChoiceQuestionId,choice.Id); //only validate if choice selected uses correct
            if (ModelState.IsValid)
            {
                db.Entry(choice).State = EntityState.Modified;
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index", "Choices", new { SelectedQuestion = choice.ChoiceQuestionId });
            }
            ViewBag.SelectedQuestion = new SelectList(db.Question, "Id", "Name", choice.ChoiceQuestionId);
            return View(choice);
        }

        // GET: Choices/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsChoiceForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Choice choice = db.Choice.Find(id);
            if (choice == null)
            {
                return HttpNotFound();
            }
            return View(choice);
        }

        // POST: Choices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsChoiceForLoggedInUser(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Choice choice = db.Choice.Find(id);
            db.Choice.Remove(choice);
            db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
            return RedirectToAction("Index", "ChoiceQuestions");
        }

        private void ValidateChoice(long questionId)
        {
            //GamePlay Business Rules
            if (ProbeValidate.IsQuestionPossessCorrectChoice(questionId))
            {
                ModelState.AddModelError("Correct", "The question can only have one correct choice.");
            }

        }

        private void ValidateChoice(long questionId, long choiceId)
        {
            //GamePlay Business Rules
            if (ProbeValidate.IsQuestionPossessCorrectChoice(questionId, choiceId))
            {
                ModelState.AddModelError("Correct", "The question can only have one correct choice.");
            }

        }

        private int GetNextOrderNbr(long SelectedQuestion)
        {
            if (db.Choice.Where(c => c.ChoiceQuestionId == SelectedQuestion).Count() > 0)
            {
                return (int)db.Choice.Where(c => c.ChoiceQuestionId == SelectedQuestion).Max(c => c.OrderNbr) + 1;
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
