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

        // GET: api/Players
        public List<PlayerDTO> GetPlayerByGamePlay(long id)
        {
            //without this command there would be a serializer error when returning the db.Players
            db.Configuration.LazyLoadingEnabled = false;
            var players = db.Player.Where(p => p.GamePlayId == id).OrderBy(p => p.FirstName + "-" + p.NickName);

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

        // GET: api/Players NOTE: currently used by server client page (gameplays)
        [Route("api/Players/GetPlayerByGameCode/{code}")]
        public List<PlayerDTO> GetPlayerByGameCode(string code)
        {
            //without this command there would be a serializer error when returning the db.Players
            db.Configuration.LazyLoadingEnabled = false;
            var players = db.Player.Where(p => p.GamePlay.Code == code).OrderBy(p => p.FirstName + "-" + p.NickName);

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

        //// POST: api/Players NOTE: currently used by the client 11/2/14 (DEPRECATED - AS OF 2/14/15)
        //[ResponseType(typeof(PlayerDTO))]
        //public IHttpActionResult PostPlayer(PlayerDTO playerDTO)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    /*
        //     * Let's make sure the gameplayid and gamecode match up correctly. check for malicious activity
        //     */
        //    try
        //    {
        //        ProbeValidate.ValidateGameCodeVersusId(playerDTO.GamePlayId, playerDTO.GameCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
        //        return BadRequest(ModelState);
        //    }

        //    /*
        //     * If we've gotten this far, then the required fields and game code security
        //     * validations have passed
        //     */
        //    try
        //    {
        //        GamePlay gp = db.GamePlay.Find(playerDTO.GamePlayId);
        //        //business validations
        //        if (!ProbeValidate.IsGamePlayActive(gp))
        //        {
        //            throw new GamePlayNotActiveException();
        //        }

        //        DateTime dateTimeNow = DateTime.Now;
        //        Player player = new Player
        //        {
        //            GamePlayId = playerDTO.GamePlayId,
        //            FirstName = playerDTO.FirstName,
        //            LastName = playerDTO.LastName,
        //            NickName = playerDTO.NickName,
        //            Sex = playerDTO.Sex,
        //            SubmitDate = dateTimeNow.Date,
        //            SubmitTime = DateTime.Parse(dateTimeNow.ToShortTimeString())
        //        };

        //        //will throw the following exceptions if there is a problem
        //        //GamePlayDuplicatePlayerNameException, GamePlayInvalidFirstNameException, GamePlayInvalidNickNameException
        //        ProbeValidate.IsGamePlayPlayerValid(gp.Id, player);

        //        db.Person.Add(player);
        //        db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

        //        playerDTO.Id = player.Id; //after db.SaveChanges. The id is set 
        //        return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO); //EVERYTHING IS GOOD!

        //    } //try
        //    catch (GamePlayNotActiveException)
        //    {
        //        var errorObject = new
        //        {
        //            errorid = 2,
        //            errormessage = "This game play is not active at this time.",
        //            gameplayid = playerDTO.GamePlayId
        //        };
        //        return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
        //    }
        //    catch (GamePlayDuplicatePlayerNameException)
        //    {
        //        var errorObject = new
        //        {
        //            errorid = 3,
        //            errormessage = "The player's name has already been used in this game.",
        //            playername = playerDTO.FirstName + '-' + playerDTO.NickName
        //        };
        //        return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
        //    }
        //    catch (GamePlayInvalidFirstNameException)
        //    {
        //        var errorObject = new
        //        {
        //            errorid = 4,
        //            errormessage = "The player's first name is invalid.",
        //            playername = playerDTO.FirstName + '-' + playerDTO.NickName
        //        };
        //        return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
        //    }
        //    catch (GamePlayInvalidNickNameException)
        //    {
        //        var errorObject = new
        //        {
        //            errorid = 5,
        //            errormessage = "The player's nick name is invalid.",
        //            playername = playerDTO.FirstName + '-' + playerDTO.NickName
        //        };
        //        return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorObject = new
        //        {
        //            errorid = ex.HResult,
        //            errormessage = ex.Message,
        //            errorinner = ex.InnerException,
        //            errortrace = ex.StackTrace
        //        };
        //        return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);

        //    }
        //}

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
                ProbeValidate.ValidateGameCodeVersusId(playerDTO.GamePlayId, playerDTO.GameCode);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                return BadRequest(ModelState);
            }

            /*
             * If we've gotten this far, then the required fields and game code security
             * validations have passed
             */
            try
            {
                GamePlay gp = db.GamePlay.Find(playerDTO.GamePlayId);
                //business validations
                if (!ProbeValidate.IsGamePlayActive(gp))
                {
                    throw new GamePlayNotActiveException();
                }

                DateTime dateTimeNow = DateTime.Now;
                Player player = new Player
                {
                    GamePlayId = playerDTO.GamePlayId,
                    FirstName = playerDTO.FirstName,
                    LastName = playerDTO.LastName,
                    NickName = playerDTO.NickName,
                    Sex = playerDTO.Sex,
                    SubmitDate = dateTimeNow.Date,
                    SubmitTime = DateTime.Parse(dateTimeNow.ToShortTimeString())
                };

                //will throw the following exceptions if there is a problem
                //GamePlayDuplicatePlayerNameException, GamePlayInvalidFirstNameException, GamePlayInvalidNickNameException
                ProbeValidate.IsGamePlayPlayerValid(gp.Id, player);

                db.Person.Add(player);
                db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

                playerDTO.Id = player.Id; //after db.SaveChanges. The id is set

                //Making this API backward compatible. Will not attempt to record game answers if its
                //client version v1.0
                if (playerDTO.ClientVersion != ProbeConstants.ClientVersionPostPlayerWithoutAnswers)
                {

                    if (playerDTO.GamePlayAnswers == null)
                    {
                        throw new PlayerDTOMissingAnswersException();
                    }

                    //DON'T NEED TO RETURN GamePlayAnswers
                    //List<GamePlayAnswerDTO> gamePlayAnswerDTOsOut = new List<GamePlayAnswerDTO>();

                    //create GamePlayAnswerDTO's (Id, PlayerId, ChoiceId)
                    foreach (GamePlayAnswer gamePlayAnswerDTO in playerDTO.GamePlayAnswers)
                    {
                        //we need to create a gamePlayAnswer (to record in the database)
                        GamePlayAnswer gamePlayAnswer = new GamePlayAnswer
                        {
                            PlayerId = playerDTO.Id,
                            ChoiceId = gamePlayAnswerDTO.ChoiceId
                        };

                        db.GamePlayAnswer.Add(gamePlayAnswer);
                        db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

                        //DON'T NEED TO RETURN GamePlayAnswers
                        //GamePlayAnswerDTO gamePlayAnswerDTOOut = new GamePlayAnswerDTO();
                        //gamePlayAnswerDTOOut.Id = gamePlayAnswer.Id;
                        //gamePlayAnswerDTOOut.PlayerId = gamePlayAnswer.PlayerId;
                        //gamePlayAnswerDTOOut.ChoiceId = gamePlayAnswer.ChoiceId;
                        //gamePlayAnswerDTOsOut.Add(gamePlayAnswerDTOOut);

                    } //foreach (GamePlayAnswerDTO gamePlayAnswerDTOIn in gamePlayAnswersDTOsIn)

                } //if (!playerDTO.ClientVersion.Contains("v1.0"))

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO); //EVERYTHING IS GOOD!

            } //try
            catch (GamePlayNotActiveException)
            {
                var errorObject = new
                {
                    errorid = 2,
                    errormessage = "This game play is not active at this time.",
                    gameplayid = playerDTO.GamePlayId
                };
                return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
            }
            catch (GamePlayDuplicatePlayerNameException)
            {
                var errorObject = new
                {
                    errorid = 3,
                    errormessage = "The player's name has already been used in this game.",
                    playername = playerDTO.FirstName + '-' + playerDTO.NickName
                };
                return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
            }
            catch (GamePlayInvalidFirstNameException)
            {
                var errorObject = new
                {
                    errorid = 4,
                    errormessage = "The player's first name is invalid.",
                    playername = playerDTO.FirstName + '-' + playerDTO.NickName
                };
                return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
            }
            catch (GamePlayInvalidNickNameException)
            {
                var errorObject = new
                {
                    errorid = 5,
                    errormessage = "The player's nick name is invalid.",
                    playername = playerDTO.FirstName + '-' + playerDTO.NickName
                };
                return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
            }
            catch (PlayerDTOMissingAnswersException)
            {
                var errorObject = new
                {
                    errorid = 6,
                    errormessage = "The client player answer submission is missing question-answers.",
                    playername = playerDTO.FirstName + '-' + playerDTO.NickName
                };
                return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
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
                return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);

            }
        }


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