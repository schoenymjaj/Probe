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

namespace Probe.Controllers.api
{
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


        // GET: api/GamePlays/GetGamePlay/{code} NOTE: currently used by client (11/2/14)
        [Route("api/GamePlays/GetGamePlay/{code}")]
        [ResponseType(typeof(GamePlay))]
        public IHttpActionResult GetGamePlay(string code)
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

        }

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