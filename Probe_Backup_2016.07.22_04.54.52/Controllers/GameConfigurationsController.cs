using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Probe.DAL;
using ProbeDAL.Models;
using Probe.Models.View;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Probe.Helpers.Validations;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Probe.Helpers.Mics;

namespace Probe.Controllers
{
    public class GameConfigurationsController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: GameConfigurations
        public ActionResult Index(long gameid)
        {
            Game game = db.Game.Find(gameid);

            ViewBag.GameId = gameid;
            ViewBag.GameName = game.Name;
            ViewBag.GameEditable = !ProbeValidate.IsGameActive(game);

            return View();
        }

        public JsonResult GetGameConfiguration([DataSourceRequest]DataSourceRequest request, long gameid)
        {
            try
            {
                //limit the games to only what the user possesses
                string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

                /*
                    * Outer join with configurationG and gameconfiguration table records
                    */
                Game game = db.Game.Find(gameid);
                long gameTypeId = game.GameTypeId;
                string gameName = game.Name;

                var gameConfigurations = db.ConfigurationG
                    .Where(c => c.ConfigurationType != ConfigurationG.ProbeConfigurationType.GLOBAL &&
                        c.GameTypeConfiguration.Any(gtc => gtc.GameTypeId == gameTypeId))
                    .SelectMany(c => c.GameConfigurations
                            .Where(gc => loggedInUserId != "-1" && gc.Game.Id == gameid).DefaultIfEmpty(),
                    (c, gc) =>
                        new GameConfigurationDTO
                        {
                            Id = (gc.Id != null) ? gc.Id : -1,
                            ConfigurationGId = c.Id,
                            GameId = gameid,
                            GameName = gameName,
                            GroupName = c.Group.Name,
                            Name = c.Name,
                            ShortDescription = c.ShortDescription,
                            Description = c.Description,
                            DataTypeG = c.DataTypeG,
                            ConfigurationType = c.ConfigurationType,
                            Value = (gc.Value != null) ? gc.Value : c.Value
                        }
                    ).OrderBy(gcX => gcX.Name);

                return this.Json(gameConfigurations.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState.ToDataSourceResult());
            }
        }//public JsonResult GetGameConfiguration([DataSourceRequest]DataSourceRequest request)

        public JsonResult GetGameConfigsForAutoComplete(long gameid)
        {
            try
            {
                //limit the games to only what the user possesses
                string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");


                Game game = db.Game.Find(gameid);
                long gameTypeId = game.GameTypeId;
                string gameName = game.Name;

                db.Configuration.LazyLoadingEnabled = false; //Need to do this if we return the entire game. If we just get the name; we probably don't.
                var gameConfigurations = db.ConfigurationG
                    .Where(c => c.ConfigurationType != ConfigurationG.ProbeConfigurationType.GLOBAL &&
                        c.GameTypeConfiguration.Any(gtc => gtc.GameTypeId == gameTypeId))
                    .SelectMany(c => c.GameConfigurations
                            .Where(gc => loggedInUserId != "-1" && gc.Game.Id == gameid).DefaultIfEmpty(),
                    (c, gc) =>
                        new 
                        {
                            Name = c.Name,
                        }
                    ).OrderBy(gcX => gcX.Name);

                var gameConfigurationNames = gameConfigurations.Select(x => x.Name);

                return Json(gameConfigurationNames, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState.ToDataSourceResult());
            }

        }//public JsonResult GetGameConfigurationForAutoComplete()

        /*
         * Get all Game Types
         */
        public JsonResult GetDataTypeGs()
        {
            try
            {
                var itemsVar = EnumHelper.SelectListFor<ConfigurationG.ProbeDataType>();
                var items = itemsVar.ToList();
                items[0].Value = "0";
                items[1].Value = "1";
                items[2].Value = "2";
                items[3].Value = "3";

                return Json(items, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState.ToDataSourceResult());
            }

        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest dsRequest, GameConfigurationDTO gameConfigurationDTO)
        {

            try
            {
                long gameId = (long)gameConfigurationDTO.GameId;

                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsGameForLoggedInUser((long)gameConfigurationDTO.GameId))
                {
                    ModelState.AddModelError("", "Game Configuration Update could not be accomplished");
                    return Json(ModelState.ToDataSourceResult());
                }

                if (ModelState.IsValid)
                {

                    switch (gameConfigurationDTO.DataTypeG)
                    {
                        case ConfigurationG.ProbeDataType.TEXT:
                            {
                                if (gameConfigurationDTO.Value.Contains("script")) {
                                    ModelState.AddModelError("Value", "Value entered is invalid");
                                    return Json(ModelState.ToDataSourceResult());
                                }
                                else if (gameConfigurationDTO.Value.Contains("$("))
                                {
                                    ModelState.AddModelError("Value", "Value entered is invalid");
                                    return Json(ModelState.ToDataSourceResult());
                                }
                                else if (gameConfigurationDTO.Value.ToLower().Contains("delete"))
                                {
                                    ModelState.AddModelError("Value", "Value entered is invalid");
                                    return Json(ModelState.ToDataSourceResult());
                                }
                                break;
                            }
                        case ConfigurationG.ProbeDataType.INT:
                            {
                                int valueOutput;
                                if (!Int32.TryParse(gameConfigurationDTO.Value, out valueOutput))
                                {
                                    ModelState.AddModelError("Value", "Value entered is not an integer");
                                    return Json(ModelState.ToDataSourceResult());
                                }
                                break;
                            }
                        case ConfigurationG.ProbeDataType.FLOAT:
                            {
                                double valueOutput;
                                if (!Double.TryParse(gameConfigurationDTO.Value, out valueOutput))
                                {
                                    ModelState.AddModelError("Value", "Value entered is not a real");
                                    return Json(ModelState.ToDataSourceResult());
                                }
                                break;
                            }
                        case ConfigurationG.ProbeDataType.BOOLEAN:
                            {
                                if (!(gameConfigurationDTO.Value.IsCaseInsensitiveEqual("false") ||
                                    gameConfigurationDTO.Value.IsCaseInsensitiveEqual("true")))
                                {
                                    ModelState.AddModelError("Value", "Value entered must be true or false");
                                    return Json(ModelState.ToDataSourceResult());
                                }
                                else
                                {
                                    gameConfigurationDTO.Value = gameConfigurationDTO.Value.ToLower();
                                }
                                break;
                            }

                    };


                    //Create a valid GameConfiguration record
                    GameConfiguration gameConfiguration = new GameConfiguration
                    {
                        ConfigurationGId = gameConfigurationDTO.ConfigurationGId,
                        GameId = (long)gameConfigurationDTO.GameId,
                        Value = gameConfigurationDTO.Value
                    };

                    /*
                     * First we will check if there is an existing GameConfiguration record. If not, then we will create that record. If
                     * it exists; then we will update that record.
                     */
                    if (db.GameConfiguration
                        .Where(gc => gc.ConfigurationGId == gameConfigurationDTO.ConfigurationGId && gc.GameId == gameConfigurationDTO.GameId).Count() == 0)
                    {
                        //GameConfiguration record doesn't exist; so we are going to create it.
                        db.GameConfiguration.Add(gameConfiguration);
                        db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                        gameConfigurationDTO.Id = gameConfiguration.Id;
                    }
                    else
                    {
                        //to edit an existing record; you need to seed the id with the existing PK
                        gameConfiguration.Id = (int)gameConfigurationDTO.Id;

                        db.Entry(gameConfiguration).State = EntityState.Modified;
                        db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                    }
                }//if (ModelState.IsValid)

                //return Json(ModelState.ToDataSourceResult());
                return Json(new[] { gameConfigurationDTO }.ToDataSourceResult(dsRequest, ModelState));

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState.ToDataSourceResult());
            }
        }//public ActionResult Update([DataSourceRequest] DataSourceRequest dsRequest, GameConfigurationDTO gameConfigurationDTO)

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
