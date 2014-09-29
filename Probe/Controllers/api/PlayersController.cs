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

        // POST: api/Players
        [ResponseType(typeof(Player))]
        public IHttpActionResult PostPlayer(Player player)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {

                GamePlay gp = db.GamePlay.Find(player.GamePlayId);
                //business validations
                if (!ProbeValidate.IsGamePlayActive(gp))
                {
                    throw new GamePlayNotActiveException();
                }

                //will throw the following exceptions if there is a problem
                //GamePlayDuplicatePlayerNameException, GamePlayInvalidFirstNameException, GamePlayInvalidNickNameException
                ProbeValidate.IsGamePlayPlayerValid(gp.Id, player);

                DateTime dateTimeNow = DateTime.Now;

                player.SubmitDate = dateTimeNow.Date;
                player.SubmitTime = DateTime.Parse(dateTimeNow.ToShortTimeString());

                db.Person.Add(player);
                db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

                //transform to player DTO for return item
                PlayerDTO playerDTO = new PlayerDTO
                {
                    Id = player.Id,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    NickName = player.NickName,
                    Sex = player.Sex
                };

                return CreatedAtRoute("DefaultApi", new { id = playerDTO.Id }, playerDTO); //EVERYTHING IS GOOD!
            }//try
            catch (GamePlayNotActiveException)
            {
                var errorObject = new
                {
                    errorid = 2,
                    errormessage = "This game play is not active at this time.",
                    gameplayid = player.GamePlayId
                };
                return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
            }
            catch (GamePlayDuplicatePlayerNameException)
            {
                var errorObject = new
                {
                    errorid = 3,
                    errormessage = "The player's name has already been used in this game.",
                    playername = player.FirstName + '-' + player.NickName
                };
                return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
            }
            catch (GamePlayInvalidFirstNameException)
            {
                var errorObject = new
                {
                    errorid = 4,
                    errormessage = "The player's first name is invalid.",
                    playername = player.FirstName + '-' + player.NickName
                };
                return CreatedAtRoute("DefaultApi", new { id = errorObject.errorid }, errorObject);
            }
            catch (GamePlayInvalidNickNameException)
            {
                var errorObject = new
                {
                    errorid = 5,
                    errormessage = "The player's nick name is invalid.",
                    playername = player.FirstName + '-' + player.NickName
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