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
using Probe.Helpers.Exceptions;
using System.Web.Http.ModelBinding;
using Probe.Helpers.ModelBinders;
using Probe.Helpers.Mics;
using Probe.Helpers.GameHelpers;
using Probe.Helpers.PlayerHelpers;

namespace Probe.Controllers.api
{
    public class PlayersController : ApiController
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: api/Players
        public IQueryable<Player> GetPlayer()
        {
            //without this command there would be a serializer error when returning the db.Players
            db.Configuration.LazyLoadingEnabled = false;
            var players = db.Player;

            return players;
        }

        // GET: api/Players NOTE: currently used by server client page (games)
        public List<PlayerDTO> GetPlayerByGame(long id)
        {
            //without this command there would be a serializer error when returning the db.Players
            db.Configuration.LazyLoadingEnabled = false;
            var players = db.Player.Where(p => p.GameId == id).OrderBy(p => p.FirstName + "-" + p.NickName);

            List<PlayerDTO> playerDTOs = new List<PlayerDTO>();

            foreach (Player player in players)
            {
                PlayerDTO playerDTO = new PlayerDTO
                {
                    Id = player.Id,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    NickName = player.NickName,
                    Sex = player.Sex
                };
                playerDTOs.Add(playerDTO);
            }

            return playerDTOs;
        }

        // GET: api/Players NOTE: currently used by server client page (games)
        [Route("api/Players/GetPlayerByGameCode/{code}")]
        public List<PlayerDTO> GetPlayerByGameCode(string code)
        {
            //without this command there would be a serializer error when returning the db.Players
            db.Configuration.LazyLoadingEnabled = false;
            var players = db.Player.Where(p => p.Game.Code == code).OrderBy(p => p.FirstName + "-" + p.NickName);

            List<PlayerDTO> playerDTOs = new List<PlayerDTO>();

            foreach (Player player in players)
            {
                PlayerDTO playerDTO = new PlayerDTO
                {
                    Id = player.Id,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    NickName = player.NickName,
                    EmailAddr = player.EmailAddr,
                    Sex = player.Sex,
                    PlayerGameName = new ProbePlayer(player).PlayerGameName
                };
                playerDTOs.Add(playerDTO);
            }

            return playerDTOs;
        }

        // GET: api/Players/5
        [ResponseType(typeof(Player))]
        public IHttpActionResult GetPlayer(long id)
        {
            Player player = db.Player.Find(id);
            if (player == null)
            {
                return NotFound();
            }

            return Ok(player);
        }


        // PUT: api/Players/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPlayer(long id, Player player)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != player.Id)
            {
                return BadRequest();
            }

            db.Entry(player).State = EntityState.Modified;

            try
            {
                db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(id))
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

        // POST: api/Players NOTE: currently used by the client 11/2/14 - NEW API AS OF 2/14/15
        [ResponseType(typeof(PlayerDTO))]
        public IHttpActionResult PostPlayer([ModelBinder(typeof(PlayerModelBinderProvider))] PlayerDTO playerDTO)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*
             * Let's make sure the gameplayid and gamecode match up correctly. check for malicious activity
             */
            try
            {
                ProbeValidate.ValidateGameCodeVersusId(playerDTO.GameId, playerDTO.GameCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                return BadRequest(ModelState);
            }

            int nbrPlayersAnswersCorrect = 0;
            DateTime dateTimeNow = DateTime.UtcNow;
            Player player = new Player();
            
            //Create a GameAnswers Collection
            ICollection<GameAnswer> gameAnswers = new List<GameAnswer>();
            foreach (GameAnswerDTO gameAnswerDTO in playerDTO.GameAnswers)
            {
                //we need to create a gameAnswer (to record in the database)
                GameAnswer gameAnswer = new GameAnswer
                {
                    PlayerId = playerDTO.Id,
                    ChoiceId = gameAnswerDTO.ChoiceId
                };
                gameAnswers.Add(gameAnswer);
            } //foreach (GameAnswerDTO gameAnswerDTO in playerDTO.GameAnswers)

            Game g = db.Game.Find(playerDTO.GameId);

            ProbeGame probeGame = new ProbeGame(g);

            /*
             * If we've gotten this far, then the required fields and game code security
             * validations have passed
             */
            try
            {
                //business validations
                if (!probeGame.IsActive())
                {
                    throw new GameNotActiveException();
                }

                player = new Player
                {
                    Id = playerDTO.Id,
                    GameId = playerDTO.GameId,
                    FirstName = playerDTO.FirstName,
                    LastName = playerDTO.LastName,
                    NickName = playerDTO.NickName,
                    EmailAddr = playerDTO.EmailAddr,
                    Sex = playerDTO.Sex,
                    //Active = true, //Do not specify at this point
                    SubmitDate = dateTimeNow.Date,
                    SubmitTime = DateTime.Parse(dateTimeNow.ToShortTimeString())
                };

                if (!probeGame.IsPlayerSubmitted(player))
                {
                    //will throw the following exceptions if there is a problem
                    //GameDuplicatePlayerNameException, GameInvalidFirstNameException, GameInvalidNickNameException
                    //ONLY NEED TO VALIDATE IF THE PLAYER HAS NOT SUBMITTED FOR A GAME YET
                    probeGame.ValidateGamePlayer(player);

                    player.Active = true; //Player is always active to begin with
                    db.Person.Add(player);
                    db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);
                    playerDTO.Id = player.Id; //after db.SaveChanges. The id is set
                }

                if (!probeGame.IsPlayerActive(player))
                {
                    throw new GamePlayerInActiveException();
                }

                //Making this API backward compatible. Will not attempt to record game answers if its
                //client version v1.0
                if (playerDTO.ClientVersion != ProbeConstants.ClientVersionPostPlayerWithoutAnswers)
                {

                    if (playerDTO.GameAnswers == null)
                    {
                        throw new PlayerDTOMissingAnswersException();
                    }
                    else if (!probeGame.IsValidGameAnswer(gameAnswers))
                    {
                        throw new InvalidGameAnswersException();
                    }

                    //Determine if the GameAnswer submission is not too early. We pass the DTO version because it has the question number
                    //Note: This audit had to come after the player is submitted check and player added to database
                    if (!probeGame.IsPlayerGameAnswerNotTooEarly(dateTimeNow, gameAnswers))
                    {
                        throw new GameAnswersTooEarlyException();
                    }

                    //Determine if the GameAnswer submission is ontime. We pass the DTO version because it has the question number
                    //Note: This audit had to come after the player is submitted check and player added to database
                    if (!probeGame.IsPlayerGameAnswerOnTime(dateTimeNow, gameAnswers))
                    {
                        throw new GameAnswersTooLateException();
                    }

                    //create GameAnswerDTO's (Id, PlayerId, ChoiceId)
                    foreach (GameAnswer gameAnswer in gameAnswers)
                    {
                        //we need to create a gameAnswer (to record in the database)
                        GameAnswer GameAnswerforDB = new GameAnswer
                        {
                            PlayerId = playerDTO.Id,
                            ChoiceId = gameAnswer.ChoiceId
                        };

                        db.GameAnswer.Add(GameAnswerforDB);
                        db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

                    } //foreach (GameAnswerDTO gameAnswerDTOIn in gameAnswersDTOsIn)

                    //We pass in the playerDO.GameAnswers because it holds the QuestionId of each question. Much
                    //more assurance that we are correcting the appropriate questions and answers
                    nbrPlayersAnswersCorrect = probeGame.NbrPlayerAnswersCorrect(playerDTO.GameAnswers);

                    //if the game is LMS - Determine if any of the answers submitted were wrong.
                    //If so then we are going to make the player inactive
                    if (probeGame.GameType == ProbeConstants.LMSGameType)
                    {
                        if (playerDTO.GameAnswers.Count() > nbrPlayersAnswersCorrect)
                        {
                            //We need to make the player inactive.
                            probeGame.SetPlayerStatus(player, false, Player.PlayerGameReasonType.ANSWER_REASON_INCORRECT);
                        }
                    }//if (probeGame.GameType == ProbeConstants.LMSGameType)

                } //if (!playerDTO.ClientVersion.Contains("v1.0"))

                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.NbrPlayers = probeGame.NbrPlayers;
                playerDTO.PlayerGameStatus.NbrPlayersRemaining = probeGame.NbrPlayersActive;
                playerDTO.PlayerGameStatus.NbrAnswersCorrect = nbrPlayersAnswersCorrect;
                playerDTO.PlayerGameStatus.PlayerActive = probeGame.IsPlayerActive(player);
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_NoError;

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO); //EVERYTHING IS GOOD!

            } //try
            catch (GameNotActiveException)
            {
                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_GameNotActive;
                playerDTO.PlayerGameStatus.Message = "This game is not active at this time.";

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO);
            }
            catch (GameDuplicatePlayerNameException)
            {
                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_PlayerDupInGame;
                string playername = new ProbePlayer(player).PlayerGameName;
                playerDTO.PlayerGameStatus.Message = "The player's name (" + playername + ") has already been used in this game.";

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO);

            }
            catch (GameInvalidFirstNameException)
            {
                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_PlayerFirstNameInvalid;
                playerDTO.PlayerGameStatus.Message = "The player's first name is invalid.";

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO);
            }
            catch (GameInvalidNickNameException)
            {
                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_PlayerNickNameInvalid;
                playerDTO.PlayerGameStatus.Message = "The player's nick name is invalid.";

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO);
            }
            catch (PlayerDTOMissingAnswersException)
            {
                //cleanup first
                if (!probeGame.IsPlayerHaveAnswers(player)) ProbeGame.DeletePlayer(db, player);

                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_SubmissionMissingAnswers;
                playerDTO.PlayerGameStatus.Message = "The client player answer submission is missing question-answers.";

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO);
            }
            catch (InvalidGameAnswersException)
            {
                //cleanup first
                if (!probeGame.IsPlayerHaveAnswers(player)) ProbeGame.DeletePlayer(db, player);

                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_SubmissionInvalidAnswers;
                playerDTO.PlayerGameStatus.Message = "The client player answer submission possess the incorrect number of question-answers.";

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO);
            }
            catch (GameInvalidPlayerNameException)
            {
                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_PlayerNameInvalid;
                playerDTO.PlayerGameStatus.Message = "The player's name is invalid.";

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO);
            }
            catch (GameInvalidLastNameException)
            {
                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_PlayerLastNameInvalid;
                playerDTO.PlayerGameStatus.Message = "The player's last name is invalid.";

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO);
            }
            catch (GameAnswersTooLateException)
            {
                //Everything is not good. The GameAnswer submission did not come in ontime. So the 
                //player will become inactive. However, we will still send player game stats to the client.
                //We need to make the player inactive.
                //Note: we want to keep the player in the datbase (as inactive) also.
                probeGame.SetPlayerStatus(player, false,Player.PlayerGameReasonType.ANSWER_REASON_DEADLINE);

                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.NbrPlayers = probeGame.NbrPlayers;
                playerDTO.PlayerGameStatus.NbrPlayersRemaining = probeGame.NbrPlayersActive;
                playerDTO.PlayerGameStatus.NbrAnswersCorrect = nbrPlayersAnswersCorrect;
                playerDTO.PlayerGameStatus.PlayerActive = probeGame.IsPlayerActive(player);
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_SubmissionNotOntime;
                playerDTO.PlayerGameStatus.Message = "The player submission was beyond the deadline.";

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO);
            }
            catch (GameAnswersTooEarlyException)
            {
                //Everything is not good. The GameAnswer submission is too early. 
                //We will still send player game stats to the client.
                //We will keep the player status active at this point.
                //Note: we want to keep the player in the datbase (active) also.
                //probeGame.SetPlayerStatus(player, false); DONT NEED THIS FOR NOW

                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.NbrPlayers = probeGame.NbrPlayers;
                playerDTO.PlayerGameStatus.NbrPlayersRemaining = probeGame.NbrPlayersActive;
                playerDTO.PlayerGameStatus.NbrAnswersCorrect = nbrPlayersAnswersCorrect;
                playerDTO.PlayerGameStatus.PlayerActive = probeGame.IsPlayerActive(player);
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_SubmissionTooEarly;
                playerDTO.PlayerGameStatus.Message = "The player submission was too early.";

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO);
            }
            catch (GamePlayerInActiveException)
            {
                playerDTO.PlayerGameStatus = new PlayerGameStatus();
                playerDTO.PlayerGameStatus.MessageId = ProbeConstants.MSG_GamePlayerInActive;
                string playername = new ProbePlayer(player).PlayerGameName;
                playerDTO.PlayerGameStatus.Message = "The player (" + playername + ") is inactive for the game";

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO);
            }
            catch (Exception ex)
            {
                //cleanup first - different type of cleanup depends on game type
                if (probeGame.GameType == ProbeConstants.LMSGameType)
                {
                    //if LMS - we only delete the player if there are no answers for that player
                    if (!probeGame.IsPlayerHaveAnswers(player)) ProbeGame.DeletePlayer(db, player);
                }
                else
                {
                    //If Match or Test, then we remove any remants of the player and her answers 
                    ProbeGame.DeletePlayer(db, player);
                }
                var errorObject = new
                {
                    errorid = ex.HResult,
                    errormessage = ex.Message,
                    errorinner = ex.InnerException,
                    errortrace = ex.StackTrace
                };
                return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
            }
        }//public IHttpActionResult PostPlayer([...

        // DELETE: api/Players/5
        [ResponseType(typeof(Player))]
        public IHttpActionResult DeletePlayer(long id)
        {
            Player player = db.Player.Find(id);
            if (player == null)
            {
                return NotFound();
            }

            db.Person.Remove(player);
            db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

            return Ok(player);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlayerExists(long id)
        {
            return db.Person.Count(e => e.Id == id) > 0;
        }
    }
}