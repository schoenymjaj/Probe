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
                var errorObject = new
                {
                    errorid = 7,
                    errormessage = "Universal game code is incorrect."               
                };

                return Ok(errorObject);
            }

            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                List<ConfigurationG> configurationsList = db.ConfigurationG
                    .Where(c => c.ConfigurationType == ConfigurationG.ProbeConfigurationType.GLOBAL).ToList();

                IQueryable<ConfigurationG> configurations = configurationsList.AsQueryable<ConfigurationG>();

                return Ok(configurations);
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

        // GET: api/GameConfigurations/5 NOTE: currently used by client (11/2/14)
        [ResponseType(typeof(GameConfiguration))]
        [Route("api/GameConfigurations/GetGameConfiguration/{code}")]
        public IHttpActionResult GetGameConfiguration(string code)
        {

            if (!ProbeValidate.IsCodeExistInProbe(code))
            {
                //GameDoesNotExistException
                var errorObject = new
                {
                    errorid = 1,
                    errormessage = "A game was not found for the code specified.",
                    code = code
                };
                return Ok(errorObject);
            }

            try
            {
                var gameConfigurations = db.ConfigurationG
                    .Where(c => c.ConfigurationType != ConfigurationG.ProbeConfigurationType.GLOBAL)
                    .SelectMany(c => c.GameConfigurations.Where(gc => gc.Game.Code == code).DefaultIfEmpty(),
                    (c, gc) =>
                        new
                        {
                            Name = c.Name,
                            //Description = c.Description,
                            DataType = c.DataTypeG,
                            //ConfigurationType = c.ConfigurationType,
                            Value = (gc.Value != null) ? gc.Value : c.Value
                        }
                    );

                return Ok(gameConfigurations);
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

        // GET: api/GameConfigurations/5 - get all game configurations for a gameId (foreign key)
        [ResponseType(typeof(GameConfiguration))]
        public IHttpActionResult GetGameConfigurationByGame(long id)
        {

            var gameConfigurations = db.ConfigurationG
                .Where(c => c.ConfigurationType != ConfigurationG.ProbeConfigurationType.GLOBAL)
                .SelectMany(c => c.GameConfigurations.Where(gc => gc.Game.Id == id).DefaultIfEmpty(),
                (c, gc) =>
                    new
                    {
                        Name = c.Name,
                        Description = c.Description,
                        DataType = c.DataTypeG,
                        ConfigurationType = c.ConfigurationType,
                        Value = (gc.Value != null) ? gc.Value : c.Value
                    }
                );

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