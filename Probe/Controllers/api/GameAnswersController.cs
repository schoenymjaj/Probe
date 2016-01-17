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
using Probe.Models.API;
using Probe.Helpers.Validations;

namespace Probe.Controllers.api
{
    public class GameAnswersController : ApiController
    {
        private ProbeDataContext db = new ProbeDataContext();

        // POST: api/GameAnswers NOTE: 10/6/15 Now Deprecated - WAS used by client 11/2/14
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