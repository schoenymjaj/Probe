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
    public class GameAnswersController : ApiController
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: api/GameAnswers
        public IQueryable<GameAnswer> GetGameAnswer()
        {
            //without this command there would be a serializer error when returning the db.Players
            db.Configuration.LazyLoadingEnabled = false;
            return db.GameAnswer;
        }

        // GET: api/GameAnswers/5
        [ResponseType(typeof(GameAnswer))]
        public IHttpActionResult GetGameAnswer(long id)
        {
            GameAnswer GameAnswer = db.GameAnswer.Find(id);
            if (GameAnswer == null)
            {
                return NotFound();
            }

            return Ok(GameAnswer);
        }

        // PUT: api/GameAnswers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGameAnswer(long id, GameAnswer GameAnswer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != GameAnswer.Id)
            {
                return BadRequest();
            }

            db.Entry(GameAnswer).State = EntityState.Modified;

            try
            {
                db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameAnswerExists(id))
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

        // POST: api/GameAnswers
        [ResponseType(typeof(GameAnswer))]
        public IHttpActionResult PostGameAnswer(GameAnswer GameAnswer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GameAnswer.Add(GameAnswer);
            db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

            return CreatedAtRoute("DefaultApi", new { id = GameAnswer.Id }, GameAnswer);
        }

        // POST: api/GameAnswers NOTE: Currently used by client 11/2/14
        [ResponseType(typeof(GameAnswerDTO))]
        public IHttpActionResult PostGameAnswers(IList<GameAnswerDTO> GameAnswersDTOsIn)
        {
            long firstPlayerId = GameAnswersDTOsIn[0].PlayerId;
            string firstGameCode = GameAnswersDTOsIn[0].GameCode;
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

                //Delete the player - if it exists and it does not have any GameAnswers
                if (!ProbeValidate.IsPlayerHaveAnyAnswers(firstPlayerId))
                {
                    Player player = db.Player.Find(firstPlayerId);
                    db.Player.Remove(player);
                    db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);
                }

                return BadRequest(ModelState);
            }

            List<GameAnswerDTO> GameAnswerDTOsOut = new List<GameAnswerDTO>();

            //create GameAnswerDTO's (Id, PlayerId, ChoiceId)
            foreach (GameAnswerDTO GameAnswerDTOIn in GameAnswersDTOsIn)
            {
                //we need to create a GameAnswer (to record in the database)
                GameAnswer GameAnswer = new GameAnswer
                {
                    PlayerId = GameAnswerDTOIn.PlayerId,
                    ChoiceId = GameAnswerDTOIn.ChoiceId
                };

                db.GameAnswer.Add(GameAnswer);
                db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

                GameAnswerDTO GameAnswerDTOOut = new GameAnswerDTO();
                GameAnswerDTOOut.Id = GameAnswer.Id;
                GameAnswerDTOOut.PlayerId = GameAnswer.PlayerId;
                GameAnswerDTOOut.ChoiceId = GameAnswer.ChoiceId;
                GameAnswerDTOsOut.Add(GameAnswerDTOOut);

            } //foreach (GameAnswerDTO GameAnswerDTOIn in GameAnswersDTOsIn)

            //returning all the GameAnswerDTOs in the list
            return CreatedAtRoute("DefaultApi", new { id = GameAnswerDTOsOut[0].Id }, GameAnswerDTOsOut);
        } //public IHttpActionResult PostGameAnswers

        // DELETE: api/GameAnswers/5
        [ResponseType(typeof(GameAnswer))]
        public IHttpActionResult DeleteGameAnswer(long id)
        {
            GameAnswer GameAnswer = db.GameAnswer.Find(id);
            if (GameAnswer == null)
            {
                return NotFound();
            }

            db.GameAnswer.Remove(GameAnswer);
            db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

            return Ok(GameAnswer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameAnswerExists(long id)
        {
            return db.GameAnswer.Count(e => e.Id == id) > 0;
        }

    }
}