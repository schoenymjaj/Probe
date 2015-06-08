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
using Probe.Helpers.QuestionHelpers;
using Probe.Helpers.Mics;

namespace Probe.Controllers
{
    public class ChoiceQuestionsController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: ChoiceQuestions
        public ActionResult Index()
        {

            //limit the questions to only what the user possesses
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            ViewBag.DctQuestionForATest = ProbeValidate.GetAllQuestionPossessCorrectChoice();

            //sort the choices of the questions
            var question = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId && !cq.UsedInGame)
                .OrderBy(cq => cq.Name)
                .Include(cq => cq.QuestionType);

            if (Session["ResultMessage"] != null)
            {
                ViewBag.ResultMessage = (ResultMessage)Session["ResultMessage"];
                Session["ResultMessage"] = null;
            }

            return View(question.ToList());
        }

        // GET: ChoiceQuestions/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsQuestionForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ChoiceQuestion choiceQuestion = db.ChoiceQuestion.Find(id);
            if (choiceQuestion == null)
            {
                return HttpNotFound();
            }
            return View(choiceQuestion);
        }

        // GET: ChoiceQuestions/Create
        public ActionResult Create()
        {
            ViewBag.QuestionTypeId = new SelectList(db.QuestionType, "Id", "Name");

            ChoiceQuestion question = new ChoiceQuestion();
            question.OneChoice = true;

            if (User.Identity.GetUserId() != null)
            {
                question.AspNetUsersId = User.Identity.GetUserId();
            }

            return View(question);
        }

        // POST: ChoiceQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,QuestionTypeId,Name,Text,OneChoice,AspNetUsersId")] ChoiceQuestion choiceQuestion)
        {
            choiceQuestion.OneChoice = true; //for some reason; disabling the OneChoice prompt in RAZOR turns this to false.. This is a hack
            choiceQuestion.UsedInGame = false;
            ValidateQuestionCreate(choiceQuestion);
            if (ModelState.IsValid)
            {
                choiceQuestion.ACLId = 1; //question will be private for now 2/15/15
                db.Question.Add(choiceQuestion);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index");
            }

            ViewBag.QuestionTypeId = new SelectList(db.QuestionType, "Id", "Name", choiceQuestion.QuestionTypeId);
            return View(choiceQuestion);
        }

        // GET: ChoiceQuestions/Clone
        public ActionResult Clone(long id)
        {
            //limit the questions to only what the user possesses
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            ProbeQuestion.CloneQuestion(this, db, false, id);

            //The message that the calling RAZOR can use
            ResultMessage resultMessage = new ResultMessage
            {
                MessageId = ProbeConstants.MSG_QuestionCloneSuccessful,
                MessageType = Helpers.Mics.MessageType.Informational,
                Message = "The question '" + db.Question.Find(id).Name + "' has been cloned successfully"
            };
            Session["ResultMessage"] = resultMessage;

            return RedirectToAction("Index");

        } //public ActionResult Clone(long id)

        // GET: ChoiceQuestions/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsQuestionForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ChoiceQuestion choiceQuestion = db.ChoiceQuestion.Find(id);
            if (choiceQuestion == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuestionTypeId = new SelectList(db.QuestionType, "Id", "Name", choiceQuestion.QuestionTypeId);
            return View(choiceQuestion);
        }

        // POST: ChoiceQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,QuestionTypeId,Name,Text,OneChoice,AspNetUsersId")] ChoiceQuestion choiceQuestion)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsQuestionForLoggedInUser(choiceQuestion.Id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            choiceQuestion.OneChoice = true; //for some reason; disabling the OneChoice prompt in RAZOR turns this to false.. This is a hack
            ValidateQuestionEdit(choiceQuestion);
            if (ModelState.IsValid)
            {
                choiceQuestion.ACLId = 1; //question will be private for now 2/15/15
                db.Entry(choiceQuestion).State = EntityState.Modified;
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index");
            }
            ViewBag.QuestionTypeId = new SelectList(db.QuestionType, "Id", "Name", choiceQuestion.QuestionTypeId);
            return View(choiceQuestion);
        }

        // GET: ChoiceQuestions/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsQuestionForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ChoiceQuestion choiceQuestion = db.ChoiceQuestion.Find(id);
            if (choiceQuestion == null)
            {
                return HttpNotFound();
            }
            return View(choiceQuestion);
        }

        // POST: ChoiceQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsQuestionForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProbeQuestion.DeleteQuestion(this, db, id);
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

        private void ValidateQuestionCreate(Question question)
        {
            //Question Business Rules
            if (ProbeValidate.IsQuestionNameExistForLoggedInUser(question.Name))
            {
                ModelState.AddModelError("Name", "The question name already exists for the logged in user.");
            }

        }
        private void ValidateQuestionEdit(Question question)
        {
            //Question Business Rules
            if (ProbeValidate.IsQuestionNameExistForLoggedInUser(question.Id,question.Name))
            {
                ModelState.AddModelError("Name", "The question name already exists for the logged in user.");
            }

        }
    }
}
