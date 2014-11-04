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
using Probe.Helpers.Validations;

namespace Probe.Controllers.api
{
    public class GamePlayAnswersController : ApiController
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: api/GamePlayAnswers
        public IQueryable<GamePlayAnswer> GetGamePlayAnswer()
        {
            //without this command there would be a serializer error when returning the db.Players
            db.Configuration.LazyLoadingEnabled = false;
            return db.GamePlayAnswer;
        }

        // GET: api/GamePlayAnswers/5
        [ResponseType(typeof(GamePlayAnswer))]
        public IHttpActionResult GetGamePlayAnswer(long id)
        {
            GamePlayAnswer gamePlayAnswer = db.GamePlayAnswer.Find(id);
            if (gamePlayAnswer == null)
            {
                return NotFound();
            }

            return Ok(gamePlayAnswer);
        }

        // PUT: api/GamePlayAnswers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGamePlayAnswer(long id, GamePlayAnswer gamePlayAnswer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != gamePlayAnswer.Id)
            {
                return BadRequest();
            }

            db.Entry(gamePlayAnswer).State = EntityState.Modified;

            try
            {
                db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GamePlayAnswerExists(id))
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

        // POST: api/GamePlayAnswers
        [ResponseType(typeof(GamePlayAnswer))]
        public IHttpActionResult PostGamePlayAnswer(GamePlayAnswer gamePlayAnswer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GamePlayAnswer.Add(gamePlayAnswer);
            db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

            return CreatedAtRoute("DefaultApi", new { id = gamePlayAnswer.Id }, gamePlayAnswer);
        }

        // POST: api/GamePlayAnswers NOTE: Currently used by client 11/2/14
        [ResponseType(typeof(GamePlayAnswerDTO))]
        public IHttpActionResult PostGamePlayAnswers(IList<GamePlayAnswerDTO> gamePlayAnswersDTOsIn)
        {
            long firstPlayerId = gamePlayAnswersDTOsIn[0].PlayerId;
            string firstGameCode = gamePlayAnswersDTOsIn[0].GameCode;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                ProbeValidate.ValidateGameCodeVersusPlayerId(firstPlayerId, firstGameCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah

                //Delete the player - if it exists and it does not have any gameplayanswers
                if (!ProbeValidate.IsPlayerHaveAnyAnswers(firstPlayerId))
                {
                    Player player = db.Player.Find(firstPlayerId);
                    db.Player.Remove(player);
                    db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);
                }

                return BadRequest(ModelState);
            }

            List<GamePlayAnswerDTO> gamePlayAnswerDTOsOut = new List<GamePlayAnswerDTO>();

            //create GamePlayAnswerDTO's (Id, PlayerId, ChoiceId)
            foreach (GamePlayAnswerDTO gamePlayAnswerDTOIn in gamePlayAnswersDTOsIn)
            {
                //we need to create a gamePlayAnswer (to record in the database)
                GamePlayAnswer gamePlayAnswer = new GamePlayAnswer
                {
                    PlayerId = gamePlayAnswerDTOIn.PlayerId,
                    ChoiceId = gamePlayAnswerDTOIn.ChoiceId
                };

                db.GamePlayAnswer.Add(gamePlayAnswer);
                db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

                GamePlayAnswerDTO gamePlayAnswerDTOOut = new GamePlayAnswerDTO();
                gamePlayAnswerDTOOut.Id = gamePlayAnswer.Id;
                gamePlayAnswerDTOOut.PlayerId = gamePlayAnswer.PlayerId;
                gamePlayAnswerDTOOut.ChoiceId = gamePlayAnswer.ChoiceId;
                gamePlayAnswerDTOsOut.Add(gamePlayAnswerDTOOut);

            } //foreach (GamePlayAnswerDTO gamePlayAnswerDTOIn in gamePlayAnswersDTOsIn)

            //returning all the GamePlayAnswerDTOs in the list
            return CreatedAtRoute("DefaultApi", new { id = gamePlayAnswerDTOsOut[0].Id }, gamePlayAnswerDTOsOut);
        } //public IHttpActionResult PostGamePlayAnswers

        // DELETE: api/GamePlayAnswers/5
        [ResponseType(typeof(GamePlayAnswer))]
        public IHttpActionResult DeleteGamePlayAnswer(long id)
        {
            GamePlayAnswer gamePlayAnswer = db.GamePlayAnswer.Find(id);
            if (gamePlayAnswer == null)
            {
                return NotFound();
            }

            db.GamePlayAnswer.Remove(gamePlayAnswer);
            db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

            return Ok(gamePlayAnswer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GamePlayAnswerExists(long id)
        {
            return db.GamePlayAnswer.Count(e => e.Id == id) > 0;
        }

    }
}