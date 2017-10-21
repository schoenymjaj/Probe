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
using ProbeDAL.Models;
using Probe.Models;
using Probe.Helpers.Exceptions;
using Probe.Helpers.Validations;
using Probe.Helpers.Mics;
using System.Text;
using Probe.Helpers.GameHelpers;

using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Probe.Controllers.api
{
    #region Public Classes to Support GetGame(code) Serialization

    public class GetGameforCodeData
    {
        public long Id { get; set; }
        public string gameType { get; set; }
        public long gId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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
        public long Id;
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

    public class JGame
    {
        public long Id;
        public string GameType;
        public string Name;
        public string Description;
        public string Code;
        public DateTime StartDate;
        public DateTime EndDate;
        public bool TestMode;
        public string GameName;
        public DateTime ServerNowDate;
        public string ServerVersion;
        public List<JGameQuestion> GameQuestions;

        public JGame()
        {
        }

    }

    #endregion

    public class GameController : ApiController
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: api/Games/GetGamesByUser/{aspnetusersid}
        [Route("api/Games/GetGamesByUser/{aspnetusersid}")]
        public IHttpActionResult GetGamesByUser(string aspnetusersid)
        {
            db.Configuration.LazyLoadingEnabled = false;

            /*
             * Given a user (organizer). We return the Games and the Player Count for each Games
             */

            try
            {
                var games = db.Game
                                 .Select(g => new
                                 {
                                     g.Id,
                                     g.AspNetUsersId,
                                     GameType = g.GameType.Name,
                                     g.Name,
                                     g.Description,
                                     g.Code,
                                     g.GameUrl,
                                     g.StartDate,
                                     g.EndDate,
                                     g.SuspendMode,
                                     g.ClientReportAccess,
                                     g.TestMode,
                                     PlayerCount = g.Players.Count()
                                 }).Where(g => g.AspNetUsersId == aspnetusersid).OrderBy(g => g.Name);

                return Ok(games);

            }
            catch
            {
                var errorObject = new
                {
                    errorid = 1,
                    errormessage = "There was an error when reading all the games for the organizer.",
                };
                return Ok(errorObject);
            }

        }//public IHttpActionResult GetGame()


        // GET: api/Games/GetGameById/{id}
        [Route("api/Games/GetGameById/{id}")]
        [ResponseType(typeof(Game))]
        public IHttpActionResult GetGameById(long id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            /*
             * Given a Game ID. We return the Game and Player Count for Game
             */

            try
            {
                var game = db.Game
                                 .Select(g => new
                                 {
                                     g.Id,
                                     GameType = g.GameType.Name,
                                     g.Name,
                                     g.Description,
                                     g.Code,
                                     g.GameUrl,
                                     g.StartDate,
                                     g.EndDate,
                                     g.SuspendMode,
                                     g.ClientReportAccess,
                                     g.TestMode,
                                     PlayerCount = g.Players.Count()
                                 }).Where(gp => gp.Id == id).Single();

                return Ok(game);

            }         
            catch
            {
                var errorObject = new
                {
                    errorid = 1,
                    errormessage = "A game was not found for the id specified.",
                    code = id
                };
                return Ok(errorObject);
            }

        }//public IHttpActionResult GetGameById(long id)

        // GET: api/Games/GetGameByCode/{code} NOTE: currently used by client (11/2/14)
        [Route("api/Games/GetGameByCode/{code}")]
        [ResponseType(typeof(Game))]
        public IHttpActionResult GetGameByCode(string code)
        {
            db.Configuration.LazyLoadingEnabled = false;

            /*
             * Given a Game code. We return the Game and Player Count for Game
             */

            try
            {
                var game = db.Game
                                 .Select(g => new
                                 {
                                     g.Id,
                                     GameType = g.GameType.Name,
                                     g.Name,
                                     g.Description,
                                     g.Code,
                                     g.GameUrl,
                                     g.StartDate,
                                     g.EndDate,
                                     g.SuspendMode,
                                     g.ClientReportAccess,
                                     g.TestMode,
                                     PlayerCount = g.Players.Count()
                                 }).Where(gp => gp.Code == code).Single();

                return Ok(game);

            }
            catch
            {
                var errorObject = new
                {
                    errorid = 1,
                    errormessage = "A game was not found for the code specified.",
                    code = code
                };
                return Ok(errorObject);
            }

        }//public IHttpActionResult GetGameByCode(string code)

        //NOTE: currently used by client (2/1/15)
        [Route("api/Games/GetGame/{code}")]
        [ResponseType(typeof(Game))]
        public HttpResponseMessage GetGame(string code)
        {
            //THIS API IS DEPRECATED, WE JUST WANT TO TELL THE PLAYER THAT THEY NEED TO INSTALL THE LATEST VERSION OF INCOMMON
            var errorObject = new
            {
                errorid = ProbeConstants.MSG_NewInCommonVersionMustBeInstalled,
                errormessage = "The game could not be found because you need to install the latest version of the In Common app from your App store (Apple or Google Play). We are sorry for the inconvenence.",
                code = code
            };
            string aResp = JsonConvert.SerializeObject(errorObject);
            var resp = new HttpResponseMessage { Content = new StringContent(aResp, System.Text.Encoding.UTF8, "application/json") };
            return resp;
 
        } //public HttpResponseMessage GetGame(string code)

        //NOTE: currently used by client (2/1/15)
        [Route("api/Games/GetGame/{code}/{clientvernumber}")]
        [ResponseType(typeof(Game))]
        public HttpResponseMessage GetGame(string code, string clientvernumber)
        {
            /*
             * Given a Game ID. We return the Game, GameQuestions, Question/ChoiceQuestion, Choices
             */

            try
            {
                var game = db.Game.Where(g => g.Code == code);

                try
                {
                    Game isGame = game.Single();

                    if (isGame.SuspendMode)
                    {
                        throw new GameInSuspendModeException();
                    }

                    if (!ProbeValidate.IsGameActive(isGame))
                    {
                        throw new GameNotActiveException();
                    }

                }
                catch (Exception ex)
                {
                    if (ex is GameDoesNotExistException)
                    {
                        throw ex;
                    }
                    else if (ex.HResult == -2146233079)
                    {
                        throw new GameDoesNotExistException();
                    }
                    else
                    {
                        throw ex;
                    }
                }

                //Go to the Database and get the Game - Questions - Choices All at Once Time
                var result = db.Database.SqlQuery<GetGameforCodeData>
                                     ("exec GetGameforCode '" + code + "'").ToList();


                var lnqGame = result.First();

                //Lets fill the Game level
                JGame jsnGame = new JGame();
                jsnGame.Id = lnqGame.Id;
                jsnGame.GameType = lnqGame.gameType;
                jsnGame.Name = lnqGame.Name;
                jsnGame.Description = lnqGame.Description;
                jsnGame.Code = lnqGame.Code;
                jsnGame.StartDate = lnqGame.StartDate;
                jsnGame.EndDate = lnqGame.EndDate;
                jsnGame.TestMode = lnqGame.TestMode;
                jsnGame.GameName = lnqGame.GameName;
                jsnGame.ServerNowDate = DateTime.UtcNow;
                jsnGame.ServerVersion = ProbeConstants.ServerVersion;
                jsnGame.GameQuestions = new List<JGameQuestion>();

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
                    jsnQuestion.Id = lnqQuestion.QuestionId;
                    jsnQuestion.Name = lnqQuestion.qName;
                    jsnQuestion.Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(lnqQuestion.qText)); //ofuscate question text
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
                        jsnChoice.Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(lnqChoice.cText)); ////ofuscate choice text
                        jsnChoice.OrderNbr = lnqChoice.cOrderNbr;
                        jsnChoice.Correct = lnqChoice.Correct;

                        //Add each choice to the current question
                        jsnQuestion.Choices.Add(jsnChoice);
                    }
                    //For every question - we create a GameQuestion, and Question object then
                    //we add this to the Game object
                    JGameQuestion jsnGameQuestion = new JGameQuestion();
                    jsnGameQuestion.Question = jsnQuestion;
                    jsnGame.GameQuestions.Add(jsnGameQuestion);

                }

                if (jsnGame == null)
                {
                    throw new Exception("Game is Corrupted");
                }

                //We will serialize the jnsGame object and all its children and then we will package it for an HTTP Response
                string aResp = JsonConvert.SerializeObject(jsnGame);
                var resp = new HttpResponseMessage { Content = new StringContent(aResp, System.Text.Encoding.UTF8, "application/json") };
                return resp;


            }
            catch (GameInSuspendModeException)
            {
                var errorObject = new
                {
                    errorid = ProbeConstants.MSG_GameInSuspendMode,
                    errormessage = "A game has been suspended.",
                    code = code
                };
                string aResp = JsonConvert.SerializeObject(errorObject);
                var resp = new HttpResponseMessage { Content = new StringContent(aResp, System.Text.Encoding.UTF8, "application/json") };
                return resp;
            }
            catch (GameDoesNotExistException)
            {
                var errorObject = new
                {
                    errorid = 1,
                    errormessage = "A game was not found for the code specified.",
                    code = code
                };
                string aResp = JsonConvert.SerializeObject(errorObject);
                var resp = new HttpResponseMessage { Content = new StringContent(aResp, System.Text.Encoding.UTF8, "application/json") };
                return resp;
            }
            catch (GameNotActiveException)
            {
                var errorObject = new
                {
                    errorid = 2,
                    errormessage = "This game is not active at this time.",
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

        } //public HttpResponseMessage GetGame(string code)

        //NOTE: currently used by client (2/1/15)
        [Route("api/Games/GetAnyGame/{code}/{clientvernumber}")]
        [ResponseType(typeof(Game))]
        public HttpResponseMessage GetAnyGame(string code, string clientvernumber)
        {
            /*
             * Given a Game ID. We return the Game, GameQuestions, Question/ChoiceQuestion, Choices
             */

            try
            {
                var game = db.Game.Where(g => g.Code == code);

                try
                {
                    Game isGame = game.Single();
                }
                catch (Exception ex)
                {
                    if (ex is GameDoesNotExistException)
                    {
                        throw ex;
                    }
                    else if (ex.HResult == -2146233079)
                    {
                        throw new GameDoesNotExistException();
                    }
                    else
                    {
                        throw ex;
                    }
                }

                //Go to the Database and get the Game - Questions - Choices All at Once Time
                var result = db.Database.SqlQuery<GetGameforCodeData>
                                     ("exec GetGameforCode '" + code + "'").ToList();


                var lnqGame = result.First();

                //Lets fill the Game level
                JGame jsnGame = new JGame();
                jsnGame.Id = lnqGame.Id;
                jsnGame.GameType = lnqGame.gameType;
                jsnGame.Name = lnqGame.Name;
                jsnGame.Description = lnqGame.Description;
                jsnGame.Code = lnqGame.Code;
                jsnGame.StartDate = lnqGame.StartDate;
                jsnGame.EndDate = lnqGame.EndDate;
                jsnGame.TestMode = lnqGame.TestMode;
                jsnGame.GameName = lnqGame.GameName;
                jsnGame.ServerNowDate = DateTime.UtcNow;
                jsnGame.ServerVersion = ProbeConstants.ServerVersion;
                jsnGame.GameQuestions = new List<JGameQuestion>();

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
                    jsnQuestion.Id = lnqQuestion.QuestionId;
                    jsnQuestion.Name = lnqQuestion.qName;
                    jsnQuestion.Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(lnqQuestion.qText)); //ofuscate question text
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
                        jsnChoice.Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(lnqChoice.cText)); ////ofuscate choice text
                        jsnChoice.OrderNbr = lnqChoice.cOrderNbr;
                        jsnChoice.Correct = lnqChoice.Correct;

                        //Add each choice to the current question
                        jsnQuestion.Choices.Add(jsnChoice);
                    }
                    //For every question - we create a GameQuestion, and Question object then
                    //we add this to the Game object
                    JGameQuestion jsnGameQuestion = new JGameQuestion();
                    jsnGameQuestion.Question = jsnQuestion;
                    jsnGame.GameQuestions.Add(jsnGameQuestion);

                }

                if (jsnGame == null)
                {
                    throw new Exception("Game is Corrupted");
                }

                //We will serialize the jnsGame object and all its children and then we will package it for an HTTP Response
                string aResp = JsonConvert.SerializeObject(jsnGame);
                var resp = new HttpResponseMessage { Content = new StringContent(aResp, System.Text.Encoding.UTF8, "application/json") };
                return resp;


            }
            catch (GameInSuspendModeException)
            {
                var errorObject = new
                {
                    errorid = ProbeConstants.MSG_GameInSuspendMode,
                    errormessage = "A game has been suspended.",
                    code = code
                };
                string aResp = JsonConvert.SerializeObject(errorObject);
                var resp = new HttpResponseMessage { Content = new StringContent(aResp, System.Text.Encoding.UTF8, "application/json") };
                return resp;
            }
            catch (GameDoesNotExistException)
            {
                var errorObject = new
                {
                    errorid = 1,
                    errormessage = "A game was not found for the code specified.",
                    code = code
                };
                string aResp = JsonConvert.SerializeObject(errorObject);
                var resp = new HttpResponseMessage { Content = new StringContent(aResp, System.Text.Encoding.UTF8, "application/json") };
                return resp;
            }
            catch (GameNotActiveException)
            {
                var errorObject = new
                {
                    errorid = 2,
                    errormessage = "This game is not active at this time.",
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

        } //public HttpResponseMessage GetAnyGame(string code)


        // PUT: api/Games/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGame(long id, Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != game.Id)
            {
                return BadRequest();
            }

            db.Entry(game).State = EntityState.Modified;

            try
            {
                db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
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

        // POST: api/Games
        [ResponseType(typeof(Game))]
        public IHttpActionResult PostGame(Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Game.Add(game);
            db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

            return CreatedAtRoute("DefaultApi", new { id = game.Id }, game);
        }

        // DELETE: api/Games/5
        [ResponseType(typeof(Game))]
        public IHttpActionResult DeleteGame(long id)
        {
            Game game = db.Game.Find(id);
            if (game == null)
            {
                return NotFound();
            }

            db.Game.Remove(game);
            db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

            return Ok(game);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameExists(long id)
        {
            return db.Game.Count(e => e.Id == id) > 0;
        }
    }
}