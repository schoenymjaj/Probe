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
using Probe.Models.View;
using Probe.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Probe.Helpers.Validations;
using Probe.Helpers.QuestionHelpers;
using Probe.Helpers.Mics;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;


namespace Probe.Controllers
{

    public class GetQuestionforSearch
    {
        public long Id { get; set; }
        public long QuestionTypeId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Tags { get; set; }
        public bool OneChoice { get; set; }
        public bool TestEnabled { get; set; }
        public int ChoicesCount { get; set; }
        public long cId { get; set; }
        public string cName { get; set; }
        public string cText { get; set; }
        public bool Correct { get; set; }
        public long OrderNbr { get; set; }
    }

    public class ChoiceQuestionsController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        public ActionResult Index()
        {
            return View();
        }

        /*
         * Supports Question Library page - Read
         */
        public JsonResult Get([DataSourceRequest]DataSourceRequest request, string questionsearch)
        {
            try
            {
                //limit the questions to only what the user possesses
                string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

                //Go to the Database and get the Game - Questions - Choices All at Once Time
                var result = db.Database.SqlQuery<GetQuestionforSearch>
                                     ("exec GetQuestionforSearch '" + loggedInUserId + "','"
                                      + questionsearch + "'").ToList();


                var questionDTOs = result
                    .GroupBy(q => new { q.Id, q.QuestionTypeId, q.Name, q.Text, q.Tags, q.OneChoice, q.TestEnabled, q.ChoicesCount })
                    .Select(q => new QuestionDTO
                    {
                        Id = q.Key.Id,
                        QuestionTypeId = q.Key.QuestionTypeId,
                        Name = q.Key.Name,
                        Text = q.Key.Text,
                        Tags = q.Key.Tags,
                        OneChoice = q.Key.OneChoice,
                        TestEnabled = q.Key.TestEnabled,
                        ChoicesCount = q.Key.ChoicesCount,

                        Choices = result.Where(c => c.Id == q.Key.Id)
                                     .Select(c => new ChoiceDTO
                                         {
                                             Id = c.cId,
                                             Name = c.cName,
                                             Text = c.cText,
                                             Correct = c.Correct,
                                             OrderNbr = c.OrderNbr
                                         }
                                     ).OrderBy(c => c.OrderNbr)
                    }).OrderBy(q => q.Name);


                return this.Json(questionDTOs.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }
        }//public JsonResult Get([DataSourceRequest]DataSourceRequest request)


        public JsonResult GetQuestionsForAutoComplete()
        {
            try
            {
                //limit the questions to only what the user possesses
                string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

                db.Configuration.LazyLoadingEnabled = false; //Need to do this if we return the entire game. If we just get the name; we probably don't.
                var questionNames = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId && !cq.UsedInGame).Select(cq => cq.Name);
                return Json(questionNames, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }

        }//public JsonResult GetQuestionsForAutoComplete()

        /*
         * Supports GameQuestion dialog page (get questions for a specific game)
         */
        public JsonResult GetQuestions([DataSourceRequest]DataSourceRequest request, long? gameId)
        {
            try
            {
                //limit the questions to only what the user possesses
                string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

                long gameTypeId = (int)db.Game.Find(gameId).GameTypeId;

                /*
                 * Get all the question names used for the game being used in the context of getting questions. 
                 */
                var questionNamesUsed = db.GameQuestion.Where(gq => gq.GameId == gameId)
                                            .Select(gq => new
                                            {
                                                Name = gq.Question.Name
                                            });

                //if game type is test or lms then we only get test enabled questions otherwise we get all questions
                //The visible flag will be false if the game already uses an unused question with the same name.
                //We also do not pull questions that do not have choices. They can't be used in a game
                var questionDTOs = db.ChoiceQuestion.Where(cq => cq.AspNetUsersId == loggedInUserId && !cq.UsedInGame
                                                                 && (((gameTypeId == (long)ProbeGameType.Test || gameTypeId == (long)ProbeGameType.LMS) 
                                                                        && cq.Choices.Any(c => c.Correct)
                                                                      )
                                                                        || (gameTypeId != (long)ProbeGameType.Test && gameTypeId != (long)ProbeGameType.LMS)
                                                                    )
                                                                 && (cq.Choices.Count() > 0)
                                                          )
                    .Select(cq => new QuestionDTO
                    {
                        Id = cq.Id,
                        QuestionTypeId = cq.QuestionTypeId,
                        Name = cq.Name,
                        Text = cq.Text,
                        TestEnabled = cq.Choices.Any(c => c.Correct),
                        ChoicesCount = cq.Choices.Count(),
                        Visible = (gameId.HasValue) ? !questionNamesUsed.Any(qnu => qnu.Name == cq.Name) : true
                    });


                return this.Json(questionDTOs.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }//public JsonResult GetQuestions([DataSourceRequest]DataSourceRequest request)

        /*
         * Supports GameQuestion dialog page (get questions for a specific game) NEW NEW NEW NEW
         */
        public JsonResult GetGameQuestions([DataSourceRequest]DataSourceRequest request, long gameId, string questionsearch)
        {
            try
            {
                //limit the questions to only what the user possesses
                string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

                long gameTypeId = (int)db.Game.Find(gameId).GameTypeId;

                /*
                 * Get all the question names used for the game being used in the context of getting questions. 
                 */
                var questionNamesUsed = db.GameQuestion.Where(gq => gq.GameId == gameId)
                                            .Select(gq => new
                                            {
                                                Name = gq.Question.Name
                                            });

                //Go to the Database and get the Game - Questions - Choices All at Once Time
                var result = db.Database.SqlQuery<GetQuestionforSearch>
                                     ("exec GetGameQuestionsforSearch '" + loggedInUserId + "','"
                                      + questionsearch + "','" + gameTypeId + "'").ToList();


                var questionDTOs = result
                    .Select(q => new QuestionDTO
                    {
                        Id = q.Id,
                        QuestionTypeId = q.QuestionTypeId,
                        Name = q.Name,
                        Text = q.Text,
                        OneChoice = q.OneChoice,
                        TestEnabled = q.TestEnabled,
                        ChoicesCount = q.ChoicesCount,
                        Visible = !questionNamesUsed.Any(qnu => qnu.Name == q.Name)
                    }).OrderBy(q => q.Name);

                return this.Json(questionDTOs.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }//public JsonResult GetQuestions([DataSourceRequest]DataSourceRequest request)



        ///*
        // * Get question details for a question specified in the filter (support GameQuestions Kendo)
        // */
        //public JsonResult GetQuestionDetails([DataSourceRequest]DataSourceRequest request)
        //{
        //    try
        //    {
        //        long questionId = Convert.ToInt64(((Kendo.Mvc.FilterDescriptor)(request.Filters[0])).Value.ToString());

        //        //questionId = request.Filters.Where(f => f.Member == "Id").SingleOrDefault().Value;

        //        db.Configuration.LazyLoadingEnabled = true;
        //        var questionDetails = db.ChoiceQuestion.Where(cq => cq.Id == questionId).Include(cq => cq.Choices)
        //            .Select(cq => new
        //            {
        //                Id = cq.Id,
        //                QuestionTypeId = cq.QuestionTypeId,
        //                Name = cq.Name,
        //                Text = cq.Text,
        //                Choices = cq.Choices
        //                .OrderBy(c => c.OrderNbr)
        //                .Select(c => new
        //                {
        //                    c.Id,
        //                    c.Name,
        //                    c.Text,
        //                    c.Correct,
        //                    c.OrderNbr
        //                }),
        //                TestEnabled = cq.Choices.Any(c => c.Correct),
        //                ChoicesCount = cq.Choices.Count(),
        //            }).SingleOrDefault();


        //        QuestionDetailsDTO questionDetailsDTO = new QuestionDetailsDTO
        //        {
        //            Id = questionDetails.Id,
        //            QuestionTypeId = questionDetails.QuestionTypeId,
        //            Name = questionDetails.Name,
        //            Text = questionDetails.Text,
        //            TestEnabled = questionDetails.TestEnabled,
        //            ChoicesCount = questionDetails.ChoicesCount
        //        };

        //        questionDetailsDTO.Choices = new List<ChoiceDTO>();
        //        foreach (var c in questionDetails.Choices)
        //        {
        //            ChoiceDTO choice = new ChoiceDTO
        //            {
        //                Id = c.Id,
        //                Name = c.Name,
        //                Text = c.Text,
        //                Correct = c.Correct,
        //                OrderNbr = c.OrderNbr
        //            };

        //            questionDetailsDTO.Choices.Add(choice);
        //        }


        //        return this.Json(questionDetailsDTO, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}//public JsonResult GetQuestionDetails([DataSourceRequest]DataSourceRequest request)

        /*
         * Get question details for a question specified in the filter (support GameQuestions Kendo)
         */
        public JsonResult GetQuestionDetails([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                long questionId = Convert.ToInt64(((Kendo.Mvc.FilterDescriptor)(request.Filters[0])).Value.ToString());

                //questionId = request.Filters.Where(f => f.Member == "Id").SingleOrDefault().Value;

                //sort the choices of the questions
                var questionDetailsDTO = db.ChoiceQuestion.Where(cq => cq.Id == questionId)
                    .Select(cq => new QuestionDTO
                    {
                        Id = cq.Id,
                        QuestionTypeId = cq.QuestionTypeId,
                        Name = cq.Name,
                        Text = cq.Text,
                        TestEnabled = cq.Choices.Any(c => c.Correct),
                        ChoicesCount = cq.Choices.Count(),
                        Choices = cq.Choices.Select(c => new ChoiceDTO
                        {
                            Id = c.Id,
                            Text = c.Text,
                            Correct = c.Correct,
                            OrderNbr = c.OrderNbr
                        }).OrderBy(c => c.OrderNbr)
                    })
                    .OrderBy(cq => cq.Name)
                    .SingleOrDefault();

                return this.Json(questionDetailsDTO, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }//public JsonResult GetQuestionDetails([DataSourceRequest]DataSourceRequest request)


        public JsonResult GetQuestionTypes()
        {
            try
            {
                var itemsVar = new SelectList(db.QuestionType, "Id", "Name");
                var items = itemsVar.ToList();

                return Json(items, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create([DataSourceRequest] DataSourceRequest request, QuestionDTO questionDTO)
        {

            try
            {
                
                ValidateQuestionCreate(questionDTO);
                if (ModelState.IsValid)
                {

                    ChoiceQuestion choiceQuestion = new ChoiceQuestion
                    {
                        Id = 0,
                        AspNetUsersId = User.Identity.GetUserId(),
                        OneChoice = true, //for some reason; disabling the OneChoice prompt in RAZOR turns this to false.. This is a hack
                        UsedInGame = false,
                        ACLId = 1,
                        QuestionTypeId = questionDTO.QuestionTypeId,
                        Name = questionDTO.Name,
                        Text = questionDTO.Text,
                        Tags = questionDTO.Tags
                    };

                    db.Question.Add(choiceQuestion);
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                    questionDTO.Id = choiceQuestion.Id; //pass back the new Id to the client
                }

                return Json(new[] { questionDTO }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.IsValid ? true : ModelState.ToDataSourceResult());
            }

        }//public JsonResult Create([DataSourceRequest] DataSourceRequest request, QuestionDTO questionDTO)

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest dsRequest, QuestionDTO questionDTO)
        {

            try
            {
                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsQuestionForLoggedInUser((long)questionDTO.Id))
                {
                    ModelState.AddModelError("", "Question Update could not be accomplished");
                    return Json(ModelState.ToDataSourceResult());
                }

                ValidateQuestionEdit(questionDTO);
                if (ModelState.IsValid)
                {
                    ChoiceQuestion choiceQuestion = db.ChoiceQuestion.Find(questionDTO.Id);

                    //choiceQuestion.AspNetUsersId - THIS IS NOT CHANGING
                    choiceQuestion.QuestionTypeId = questionDTO.QuestionTypeId;
                    choiceQuestion.Name = questionDTO.Name;
                    choiceQuestion.Text = questionDTO.Text;
                    choiceQuestion.Tags = questionDTO.Tags;
                    //choiceQuestion.OneChoice - THIS IS NOT CHANGING
                    //choiceQuestion.UsedInGame - THIS IS NOT CHANGING
                    //choiceQuestion.ACLId - THIS IS NOT CHANGING

                    db.Entry(choiceQuestion).State = EntityState.Modified;
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                }

                //return Json(ModelState.ToDataSourceResult());
                return Json(new[] { questionDTO }.ToDataSourceResult(dsRequest, ModelState));


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }
        }//public ActionResult Update([DataSourceRequest] DataSourceRequest dsRequest, QuestionDTO questionDTO)

        // GET: ChoiceQuestions/Clone
        public JsonResult Clone(long id)
        {
            try
            {
                //limit the questions to only what the user possesses
                string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

                Dictionary<long, long> choiceXreference = new Dictionary<long, long>();
                long clonedQuestionId = ProbeQuestion.CloneQuestion(this, db, false, id, ref choiceXreference);

                //The message that the calling RAZOR can use
                ResultMessage resultMessage = new ResultMessage
                {
                    MessageId = ProbeConstants.MSG_QuestionCloneSuccessful,
                    MessageType = Helpers.Mics.MessageType.Informational,
                    Message = @"The question <span style=""font-style:italic;font-weight:bold"">" +
                    db.ChoiceQuestion.Find(id).Name + @"</span> has been cloned successfully to question <span style=""font-style:italic;font-weight:bold"">" +
                    db.ChoiceQuestion.Find(clonedQuestionId).Name + @"</span>"
                };

                NotifyProbe.NotifyQuestionChanged(User.Identity.Name); //let all clients know where was a game change.

                return Json(resultMessage, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                //The message return via an AJAX call
                ResultMessage resultMessage = new ResultMessage
                {
                    MessageId = ProbeConstants.MSG_UnsuccessfulOperation,
                    MessageType = Helpers.Mics.MessageType.Error,
                    Message = "The was an error when attempting to clone the question '" +
                    db.ChoiceQuestion.Find(id).Name + "'. " + ex.Message
                };

                return Json(resultMessage, JsonRequestBehavior.AllowGet);
            }

        } //public JsonResult Clone(long id)

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Delete([DataSourceRequest] DataSourceRequest request, GameDTO questionDTO)
        {
            try
            {
                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsQuestionForLoggedInUser((long)questionDTO.Id))
                {
                    ModelState.AddModelError("", "Question Delete could not be accomplished");
                    return Json(ModelState.ToDataSourceResult());
                }

                if (questionDTO != null && ModelState.IsValid)
                {
                    ProbeQuestion.DeleteQuestion(this, db, questionDTO.Id);
                }

                return Json(ModelState.IsValid ? true : ModelState.ToDataSourceResult());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }

        }//public JsonResult Delete([DataSourceRequest] DataSourceRequest request, QuestionDTO questionDTO)

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ValidateQuestionCreate(QuestionDTO questionDTO)
        {
            //Question Business Rules
            if (ProbeValidate.IsQuestionNameExistForLoggedInUser(questionDTO.Name))
            {
                ModelState.AddModelError("Name", "The question name already exists for the logged in user.");
            }

        }
        private void ValidateQuestionEdit(QuestionDTO questionDTO)
        {
            //Question Business Rules
            if (ProbeValidate.IsQuestionNameExistForLoggedInUser(questionDTO.Id, questionDTO.Name))
            {
                ModelState.AddModelError("Name", "The question name already exists for the logged in user.");
            }

        }
    }
}
