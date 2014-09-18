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

        // POST: api/GamePlayAnswers
        [ResponseType(typeof(GamePlayAnswer))]
        public IHttpActionResult PostGamePlayAnswers(IList<GamePlayAnswer> gamePlayAnswers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //create GamePlayAnswerDTO's (Id, PlayerId, ChoiceId)
            List<GamePlayAnswerDTO> gamePlayAnswerDTOs = new List<GamePlayAnswerDTO>();
            foreach (GamePlayAnswer gamePlayAnswer in gamePlayAnswers)
            {
                db.GamePlayAnswer.Add(gamePlayAnswer);
                db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

                GamePlayAnswerDTO gamePlayAnswerDTO = new GamePlayAnswerDTO();
                gamePlayAnswerDTO.Id = gamePlayAnswer.Id;
                gamePlayAnswerDTO.PlayerId = gamePlayAnswer.PlayerId;
                gamePlayAnswerDTO.ChoiceId = gamePlayAnswer.ChoiceId;
                gamePlayAnswerDTOs.Add(gamePlayAnswerDTO);

            }


            //returning all the GamePlayAnswerDTOs in the list
            return CreatedAtRoute("DefaultApi", new { id = gamePlayAnswerDTOs[0].Id }, gamePlayAnswerDTOs);
        }

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