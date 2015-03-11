using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Probe.DAL;
using Probe.Models;
using Probe.Helpers.Exceptions;
using Probe.Helpers.Validations;

using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Probe.Controllers.api
{
    #region Public Classes to Support GetGamePlay(code) Serialization

    public class GetGamePlayforCodeData
    {
        public long gpId { get; set; }
        public string gameType { get; set; }
        public long gId { get; set; }
        public string gpName { get; set; }
        public string gpDescription { get; set; }
        public string Code { get; set; }
        public DateTime gpStartDate { get; set; }
        public DateTime gpEndDate { get; set; }
        public bool TestMode { get; set; }
        public string GameName { get; set; }
        public long QuestionId { get; set; }
        public string qName { get; set; }
        public string qText { get; set; }
        public string QuestionType { get; set; }
        public bool OneChoice { get; set; }
        public int gqWeight { get; set; }
        public long gqOrderNbr { get; set; }
        public long cId { get; set; }
        public string cName { get; set; }
        public string cText { get; set; }
        public bool Correct { get; set; }
        public long cOrderNbr { get; set; }   
    }

    public class JChoice
    {
        public long Id;
        public string Name;
        public string Text;
        public bool Correct;
        public long OrderNbr;

        public JChoice()
        {
        }
    }

    public class JQuestion
    {
        public string Name;
        public string Text;
        public string QuestionType;
        public bool OneChoice;
        public List<JChoice> Choices;
        public int Weight;
        public long OrderNbr;

        public JQuestion()
        {
        }
    }


    public class JGameQuestion
    {
        public JQuestion Question;

        public JGameQuestion()
        {
        }
    }

    public class JGamePlay
    {
        public long Id;
        public string GameType;
        public long GameId;
        public string Name;
        public string Description;
        public string Code;
        public DateTime StartDate;
        public DateTime EndDate;
        public bool TestMode;
        public string GameName;
        public List<JGameQuestion> GameQuestions;

        public JGamePlay()
        {
        }

    }

    #endregion

    public class GamePlaysController : ApiController
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: api/GamePlays
        public IQueryable<GamePlay> GetGamePlay()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.GamePlay;
        }

        // GET: api/GamePlays/GetGamePlayById/{id}
        [Route("api/GamePlays/GetGamePlayById/{id}")]
        [ResponseType(typeof(GamePlay))]
        public IHttpActionResult GetGamePlayById(long id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            /*
             * Given a GamePlay ID. We return the GamePlay and Player Count for GamePlay
             */

            try
            {
                var gamePlay = db.GamePlay
                                 .Select(gp => new
                                 {
                                     gp.Id,
                                     gp.GameId,
                                     gp.Name,
                                     gp.Description,
                                     gp.Code,
                                     gp.GameUrl,
                                     gp.StartDate,
                                     gp.EndDate,
                                     gp.SuspendMode,
                                     gp.ClientReportAccess,
                                     gp.TestMode,
                                     PlayerCount = gp.Players.Count()
                                 }).Where(gp => gp.Id == id).Single();

                return Ok(gamePlay);

            }         
            catch
            {
                var errorObject = new
                {
                    errorid = 1,
                    errormessage = "A game play was not found for the id specified.",
                    code = id
                };
                return Ok(errorObject);
            }

        }//public IHttpActionResult GetGamePlayById(long id)

        // GET: api/GamePlays/GetGamePlayByCode/{code} NOTE: currently used by client (11/2/14)
        [Route("api/GamePlays/GetGamePlayByCode/{code}")]
        [ResponseType(typeof(GamePlay))]
        public IHttpActionResult GetGamePlayByCode(string code)
        {
            db.Configuration.LazyLoadingEnabled = false;

            /*
             * Given a GamePlay code. We return the GamePlay and Player Count for GamePlay
             */

            try
            {
                var gamePlay = db.GamePlay
                                 .Select(gp => new
                                 {
                                     gp.Id,
                                     gp.GameId,
                                     gp.Name,
                                     gp.Description,
                                     gp.Code,
                                     gp.GameUrl,
                                     gp.StartDate,
                                     gp.EndDate,
                                     gp.SuspendMode,
                                     gp.ClientReportAccess,
                                     gp.TestMode,
                                     PlayerCount = gp.Players.Count()
                                 }).Where(gp => gp.Code == code).Single();

                return Ok(gamePlay);

            }
            catch
            {
                var errorObject = new
                {
                    errorid = 1,
                    errormessage = "A game play was not found for the code specified.",
                    code = code
                };
                return Ok(errorObject);
            }

        }//public IHttpActionResult GetGamePlayByCode(string code)


        ////MNS Deprecated API - 3-10-15
        //// GET: api/GamePlays/GetGamePlay/{code} NOTE: currently used by client (11/2/14)
        [Route("api/GamePlays/GetGamePlayDep/{code}")]
        [ResponseType(typeof(GamePlay))]
        public IHttpActionResult GetGamePlayDep(string code)
        {
            db.Configuration.LazyLoadingEnabled = false;

            /*
             * Given a GamePlay ID. We return the GamePlay, Game, GameQuestions, Question/ChoiceQuestion, Choices
             */


            try
            {
                var gamePlay = db.GamePlay
                                 .Where(gp => gp.Code == code);

                try
                {
                    GamePlay isGamePlay = gamePlay.Single();
                    if (!ProbeValidate.IsGamePlayActive(isGamePlay))
                    {
                        throw new GamePlayNotActiveException();
                    }
                }
                catch (Exception ex)
                {
                    if (ex is GamePlayDoesNotExistException)
                    {
                        throw ex;
                    }
                    else if (ex.HResult == -2146233079)
                    {
                        throw new GamePlayDoesNotExistException();
                    }
                    else
                    {
                        throw ex;
                    }
                }

                var gamePlayResponse = gamePlay
                                .Include(gp => gp.Game)
                                .Include(gp => gp.Game.GameQuestions)
                                .Select(gp => new
                                {
                                    gp.Id,
                                    GameType = gp.Game.GameType.Name,
                                    GameId = gp.Game.Id,
                                    gp.Name,
                                    gp.Description,
                                    gp.Code,
                                    gp.StartDate,
                                    gp.EndDate,
                                    gp.TestMode,
                                    GameName = gp.Game.Name,
                                    GameQuestions = gp.Game.GameQuestions
                                                    .OrderBy(gq => gq.OrderNbr)
                                                    .Select(gq => new
                                                    {
                                                        Question = db.ChoiceQuestion.Where(cq => cq.Id == gq.Question.Id)
                                                        .Select(cq => new
                                                        {
                                                            cq.Name,
                                                            cq.Text,
                                                            QuestionType = cq.QuestionType.Name,
                                                            cq.OneChoice,
                                                            Choices = cq.Choices
                                                            .OrderBy(c => c.OrderNbr)
                                                            .Select(c => new
                                                            {
                                                                c.Id,
                                                                c.Name,
                                                                c.Text,
                                                                c.Correct,
                                                                c.OrderNbr
                                                            })
                                                        }).FirstOrDefault(),
                                                        gq.Weight,
                                                        gq.OrderNbr
                                                    })
                                }).Single();




                if (gamePlayResponse == null)
                {
                    return NotFound();
                }

                return Ok(gamePlayResponse);

            }
            catch (GamePlayDoesNotExistException)
            {
                var errorObject = new
                {
                    errorid = 1,
                    errormessage = "A game play was not found for the code specified.",
                    code = code
                };
                return Ok(errorObject);
            }
            catch (GamePlayNotActiveException)
            {
                var errorObject = new
                {
                    errorid = 2,
                    errormessage = "This game play is not active at this time.",
                    code = code
                };
                return Ok(errorObject);
            }
            catch (Exception ex)
            {
                var errorObject = new
                {
                    errorid = ex.HResult,
                    errormessage = ex.Message,
                    errorinner = ex.InnerException,
                    errortrace = ex.StackTrace
                };
                return Ok(errorObject);

            }

        } //public IHttpActionResult GetGamePlayDep(string code)

        // GET: api/GamePlays/GetGamePlay/{code} NOTE: currently used by client (11/2/14) - updated big time on 3/10/15 MNS
        [Route("api/GamePlays/GetGamePlay/{code}")]
        [ResponseType(typeof(GamePlay))]
        public HttpResponseMessage GetGamePlay(string code)
        {
            /*
             * Given a GamePlay ID. We return the GamePlay, Game, GameQuestions, Question/ChoiceQuestion, Choices
             */

            try
            {
                var gamePlay = db.GamePlay
                                 .Where(gp => gp.Code == code);

                try
                {
                    GamePlay isGamePlay = gamePlay.Single();
                    if (!ProbeValidate.IsGamePlayActive(isGamePlay))
                    {
                        throw new GamePlayNotActiveException();
                    }
                }
                catch (Exception ex)
                {
                    if (ex is GamePlayDoesNotExistException)
                    {
                        throw ex;
                    }
                    else if (ex.HResult == -2146233079)
                    {
                        throw new GamePlayDoesNotExistException();
                    }
                    else
                    {
                        throw ex;
                    }
                }

                //Go to the Database and get the GamePlay - Questions - Choices All at Once Time
                var result = db.Database.SqlQuery<GetGamePlayforCodeData>
                                     ("exec GetGamePlayforCode '" + code + "'").ToList();


                var lnqGamePlay = result.First(); 

                //Lets fill the GamePlay level
                JGamePlay jsnGamePlay = new JGamePlay();
                jsnGamePlay.Id = lnqGamePlay.gpId;
                jsnGamePlay.GameType = lnqGamePlay.gameType;
                jsnGamePlay.GameId = lnqGamePlay.gId;
                jsnGamePlay.Name = lnqGamePlay.gpName;
                jsnGamePlay.Description = lnqGamePlay.gpDescription;
                jsnGamePlay.Code = lnqGamePlay.Code;
                jsnGamePlay.StartDate = lnqGamePlay.gpStartDate;
                jsnGamePlay.EndDate = lnqGamePlay.gpEndDate;
                jsnGamePlay.TestMode = lnqGamePlay.TestMode;
                jsnGamePlay.GameName = lnqGamePlay.GameName;
                jsnGamePlay.GameQuestions = new List<JGameQuestion>();

                var lnqQuestions = result.OrderBy(q => q.gqOrderNbr).Select(q => new
                    {
                        q.QuestionId,
                        q.qName,
                        q.qText,
                        q.QuestionType,
                        q.OneChoice,
                        q.gqWeight,
                        q.gqOrderNbr
                    }).Distinct().ToList();


                foreach (var lnqQuestion in lnqQuestions)
                {
                    JQuestion jsnQuestion = new JQuestion();
                    jsnQuestion.Name = lnqQuestion.qName;
                    jsnQuestion.Text = lnqQuestion.qText;
                    jsnQuestion.OrderNbr = lnqQuestion.gqOrderNbr;
                    jsnQuestion.Weight = lnqQuestion.gqWeight;
                    jsnQuestion.QuestionType = lnqQuestion.QuestionType;
                    jsnQuestion.OneChoice = lnqQuestion.OneChoice;
                    jsnQuestion.Choices = new List<JChoice>();

                    var lnqChoices = result.Where(c => c.QuestionId == lnqQuestion.QuestionId).OrderBy(c => c.cOrderNbr)
                        .Select(c => new
                        {
                            c.cId,
                            c.cName,
                            c.cText,
                            c.Correct,
                            c.cOrderNbr
                        }).ToList();

                    foreach (var lnqChoice in lnqChoices)
                    {
                        JChoice jsnChoice = new JChoice();
                        jsnChoice.Id = lnqChoice.cId;
                        jsnChoice.Name = lnqChoice.cName;
                        jsnChoice.Text = lnqChoice.cText;
                        jsnChoice.OrderNbr = lnqChoice.cOrderNbr;
                        jsnChoice.Correct = lnqChoice.Correct;

                        //Add each choice to the current question
                        jsnQuestion.Choices.Add(jsnChoice);
                    }
                    //For every question - we create a GameQuestion, and Question object then
                    //we add this to the GamePlay object
                    JGameQuestion jsnGameQuestion = new JGameQuestion();
                    jsnGameQuestion.Question = jsnQuestion;
                    jsnGamePlay.GameQuestions.Add(jsnGameQuestion);

                }

                if (jsnGamePlay == null)
                {
                    throw new Exception("Game Play is Corrupted");
                }

                //We will serialize the jnsGamePlay object and all its children and then we will package it for an HTTP Response
                string aResp = JsonConvert.SerializeObject(jsnGamePlay);
                var resp = new HttpResponseMessage { Content = new StringContent(aResp, System.Text.Encoding.UTF8, "application/json") };
                return resp;


            }
            catch (GamePlayDoesNotExistException)
            {
                var errorObject = new
                {
                    errorid = 1,
                    errormessage = "A game play was not found for the code specified.",
                    code = code
                };
                string aResp = JsonConvert.SerializeObject(errorObject);
                var resp = new HttpResponseMessage { Content = new StringContent(aResp, System.Text.Encoding.UTF8, "application/json") };
                return resp;
            }
            catch (GamePlayNotActiveException)
            {
                var errorObject = new
                {
                    errorid = 2,
                    errormessage = "This game play is not active at this time.",
                    code = code
                };
                string aResp = JsonConvert.SerializeObject(errorObject);
                var resp = new HttpResponseMessage { Content = new StringContent(aResp, System.Text.Encoding.UTF8, "application/json") };
                return resp;
            }
            catch (Exception ex)
            {
                var errorObject = new
                {
                    errorid = ex.HResult,
                    errormessage = ex.Message,
                    errorinner = ex.InnerException,
                    errortrace = ex.StackTrace
                };
                string aResp = JsonConvert.SerializeObject(errorObject);
                var resp = new HttpResponseMessage { Content = new StringContent(aResp, System.Text.Encoding.UTF8, "application/json") };
                return resp;

            }

        } //public HttpResponseMessage GetGamePlay(string code)


        // PUT: api/GamePlays/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGamePlay(long id, GamePlay gamePlay)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != gamePlay.Id)
            {
                return BadRequest();
            }

            db.Entry(gamePlay).State = EntityState.Modified;

            try
            {
                db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GamePlayExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/GamePlays
        [ResponseType(typeof(GamePlay))]
        public IHttpActionResult PostGamePlay(GamePlay gamePlay)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GamePlay.Add(gamePlay);
            db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

            return CreatedAtRoute("DefaultApi", new { id = gamePlay.Id }, gamePlay);
        }

        // DELETE: api/GamePlays/5
        [ResponseType(typeof(GamePlay))]
        public IHttpActionResult DeleteGamePlay(long id)
        {
            GamePlay gamePlay = db.GamePlay.Find(id);
            if (gamePlay == null)
            {
                return NotFound();
            }

            db.GamePlay.Remove(gamePlay);
            db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

            return Ok(gamePlay);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GamePlayExists(long id)
        {
            return db.GamePlay.Count(e => e.Id == id) > 0;
        }
    }
}