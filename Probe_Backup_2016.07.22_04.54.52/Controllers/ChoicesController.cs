using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Probe.DAL;
using ProbeDAL;
using ProbeDAL.Models;
using Probe.Models.View;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Probe.Helpers.Validations;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Probe.Helpers.Mics;

namespace Probe.Controllers
{
    public class ChoicesController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: Choices
        public ActionResult Index(long questionid)
        {
            ChoiceQuestion question = db.ChoiceQuestion.Find(questionid);

            ViewBag.QuestionId = questionid;
            ViewBag.QuestionName = question.Name;
            ViewBag.QuestionText = question.Text;

            return View();
        }

        public JsonResult GetQuestionChoices([DataSourceRequest]DataSourceRequest request, long questionid)
        {
            try
            {
                var choices = db.Choice.Where(c => c.ChoiceQuestionId == questionid)
                    .Select(c => new ChoiceDTO
                    {
                        Id = c.Id,
                        ChoiceQuestionId = c.ChoiceQuestionId,
                        ACLId = c.ChoiceQuestion.ACLId,
                        Name = c.Name,
                        Text = c.Text,
                        Correct = c.Correct,
                        OrderNbr = c.OrderNbr
                    })
                    .OrderBy(c => c.OrderNbr);


                return this.Json(choices.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState.ToDataSourceResult());
            }
        }//public JsonResult Get([DataSourceRequest]DataSourceRequest request)

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create([DataSourceRequest] DataSourceRequest request, ChoiceDTO choiceDTO)
        {

            try
            {
                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsQuestionForLoggedInUser(choiceDTO.ChoiceQuestionId))
                {
                    ModelState.AddModelError("", "Question Create could not be accomplished");
                    return Json(ModelState.ToDataSourceResult());
                }

                if (choiceDTO.Correct) ValidateChoice(choiceDTO.ChoiceQuestionId); //only validate if choice selected uses correct

                //Set choice name - same as question name
                choiceDTO.Name = db.ChoiceQuestion.Find(choiceDTO.ChoiceQuestionId).Name;
                if (ModelState.IsValid)
                {

                    Choice choice = new Choice
                    {
                        Id = choiceDTO.Id,
                        ChoiceQuestionId = choiceDTO.ChoiceQuestionId,
                        Name = choiceDTO.Name,
                        Text = choiceDTO.Text,
                        Correct = choiceDTO.Correct,
                        OrderNbr = choiceDTO.OrderNbr
                    };
                    db.Choice.Add(choice);
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                    choiceDTO.Id = choice.Id;
                }

                NotifyProbe.NotifyChoiceChanged(User.Identity.Name); //let all clients know where was a game change.

                return Json(new[] { choiceDTO }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState.IsValid ? true : ModelState.ToDataSourceResult());
            }

        }//public JsonResult Create([DataSourceRequest] DataSourceRequest request, ChoiceDTO choiceDTO)

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest dsRequest, ChoiceDTO choiceDTO)
        {

            try
            {

                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsChoiceForLoggedInUser(choiceDTO.Id))
                {
                    ModelState.AddModelError("", "Question Update could not be accomplished");
                    return Json(ModelState.ToDataSourceResult());
                }

                if (choiceDTO.Correct) ValidateChoice(choiceDTO.ChoiceQuestionId, choiceDTO.Id); //only validate if choice selected uses correct
                if (ModelState.IsValid)
                {
                    Choice choice = new Choice
                    {
                        Id = choiceDTO.Id,
                        ChoiceQuestionId =  choiceDTO.ChoiceQuestionId,
                        Name = choiceDTO.Name,
                        Text = choiceDTO.Text,
                        Correct = choiceDTO.Correct,
                        OrderNbr = choiceDTO.OrderNbr
                    };

                    db.Entry(choice).State = EntityState.Modified;
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                    NotifyProbe.NotifyChoiceChanged(User.Identity.Name); //let all clients know where was a game change.
                }

                //return Json(ModelState.ToDataSourceResult());
                return Json(new[] { choiceDTO }.ToDataSourceResult(dsRequest, ModelState));


            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState.ToDataSourceResult());
            }
        }//public ActionResult Update([DataSourceRequest] DataSourceRequest dsRequest, ChoiceDTO choiceDTO)

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Delete([DataSourceRequest] DataSourceRequest request, ChoiceDTO choiceDTO)
        {
            try
            {
                long questionId = choiceDTO.ChoiceQuestionId;

                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsChoiceForLoggedInUser(choiceDTO.Id))
                {
                    ModelState.AddModelError("", "Choice Delete could not be accomplished");
                    return Json(ModelState.ToDataSourceResult());
                }

                if (choiceDTO != null && ModelState.IsValid)
                {
                    Choice choice = db.Choice.Find(choiceDTO.Id);
                    db.Choice.Remove(choice);
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                }

                NotifyProbe.NotifyChoiceChanged(User.Identity.Name); //let all clients know where was a game change.

                return Json(ModelState.IsValid ? true : ModelState.ToDataSourceResult());
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState.ToDataSourceResult());
            }

        }//public JsonResult Delete([DataSourceRequest] DataSourceRequest request, ChoiceDTO choiceDTO)

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
