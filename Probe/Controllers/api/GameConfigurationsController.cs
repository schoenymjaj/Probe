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
    public class GameConfigurationsController : ApiController
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: api/GameConfigurations
        [ResponseType(typeof(GameConfiguration))]
        [Route("api/GameConfigurations/GetConfiguration/{code}")]
        public IHttpActionResult GetConfiguration(string code)
        {

            if (code != "incommon-code-around")
            {
                return NotFound();
            }

            List<GameConfiguration> gameConfigurationsList = new List<GameConfiguration>
            {
                new GameConfiguration {
                    GameId = 0,
                    Name = "InCommon-About",
                    Description = "InCommon-About",
                    Value = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(@"~/Content/AboutContent.html"))
                },
                new GameConfiguration {
                    GameId = 0,
                    Name = "InCommon-CacheMinutes",
                    Description = "InCommon-CacheMinutes",
                    Value = "30"
                }
            };

            IQueryable<GameConfiguration> gameConfigurations = gameConfigurationsList.AsQueryable<GameConfiguration>();

            return Ok(gameConfigurations);
        }

        // GET: api/GameConfigurations/5 NOTE: currently used by client (11/2/14)
        [ResponseType(typeof(GameConfiguration))]
        [Route("api/GameConfigurations/GetGameConfiguration/{code}")]
        public IHttpActionResult GetGameConfiguration(string code)
        {
            //without this command there would be a serializer error when returning the db.Players
            db.Configuration.LazyLoadingEnabled = false;

            var gameConfigurations = db.GameConfiguration.Where(gc => gc.GameId == db.GamePlay.Where(gp => gp.Code == code).FirstOrDefault().GameId);
            if (gameConfigurations.Count() == 0)
            {
                return NotFound();
            }

            return Ok(gameConfigurations); //we are getting the first only because there is one config at the moment
        }

        // GET: api/GameConfigurations/5 - get all game configurations for a gameId (foreign key)
        [ResponseType(typeof(GameConfiguration))]
        public IHttpActionResult GetGameConfigurationByGame(long id)
        {
            //without this command there would be a serializer error when returning the db.Players
            db.Configuration.LazyLoadingEnabled = false;
            var gameConfigurations = db.GameConfiguration.Where(gc => gc.GameId == id);
            if (gameConfigurations == null)
            {
                return NotFound();
            }

            return Ok(gameConfigurations);
        }

        // PUT: api/GameConfigurations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGameConfiguration(long id, GameConfiguration gameConfiguration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != gameConfiguration.Id)
            {
                return BadRequest();
            }

            db.Entry(gameConfiguration).State = EntityState.Modified;

            try
            {
                db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameConfigurationExists(id))
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

        // POST: api/GameConfigurations
        [ResponseType(typeof(GameConfiguration))]
        public IHttpActionResult PostGameConfiguration(GameConfiguration gameConfiguration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GameConfiguration.Add(gameConfiguration);
            db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

            return CreatedAtRoute("DefaultApi", new { id = gameConfiguration.Id }, gameConfiguration);
        }

        // DELETE: api/GameConfigurations/5
        [ResponseType(typeof(GameConfiguration))]
        public IHttpActionResult DeleteGameConfiguration(long id)
        {
            GameConfiguration gameConfiguration = db.GameConfiguration.Find(id);
            if (gameConfiguration == null)
            {
                return NotFound();
            }

            db.GameConfiguration.Remove(gameConfiguration);
            db.SaveChanges(Request != null ? Request.Headers.UserAgent.ToString() : null);

            return Ok(gameConfiguration);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameConfigurationExists(long id)
        {
            return db.GameConfiguration.Count(e => e.Id == id) > 0;
        }
    }
}