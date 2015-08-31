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
using Probe.Helpers.Mics;
using Probe.Helpers.GameHelpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Probe.Helpers.PlayerHelpers;

namespace Probe.Controllers
{
    public class PlayersController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();
         
        // GET: Players
        public ActionResult Index(long gameid)
        {
            Game game = db.Game.Find(gameid);

            ViewBag.GameId = gameid;
            ViewBag.GameName = game.Name;
            ViewBag.GameEditable = !ProbeValidate.IsGameActive(game);

            return View();
        }

        public JsonResult GetGamePlayers([DataSourceRequest]DataSourceRequest request, long? gameid)
        {
            try
            {

                //sort the choices of the questions
                string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

                var playerDTOList = db.Player.Where(p => p.GameId == gameid || !gameid.HasValue)
                    .Select(p => new PlayerDTO
                    {
                        Id = p.Id,
                        GameId = p.GameId,
                        FirstName = p.FirstName,
                        MiddleName = p.MiddleName,
                        LastName = p.LastName,
                        NickName = p.NickName,
                        Sex = p.Sex,
                        MobileNbr = p.MobileNbr,
                        EmailAddr = p.EmailAddr,
                        Active = p.Active,
                        SubmitDate = p.SubmitDate,
                        SubmitTime = p.SubmitTime
                    }).ToList();

                /* WHEN WE PASS A LIST TO KENDO GRID - IT TAKES CARE OF CONVERTING UTC DATE TO LOCAL*/
                ////We want covert to local time and then we combine submit date and time into one DateTime prop
                foreach (PlayerDTO playerDTO in playerDTOList)
                {
                    playerDTO.SubmitDate = ClientTimeZoneHelper.ConvertToLocalTime(playerDTO.SubmitDate);
                    playerDTO.SubmitTime = ClientTimeZoneHelper.ConvertToLocalTime(playerDTO.SubmitTime);

                    playerDTO.SubmitDateTime = playerDTO.SubmitDate.Date + playerDTO.SubmitTime.TimeOfDay;
                }

                return this.Json(playerDTOList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }
        }//public JsonResult Get([DataSourceRequest]DataSourceRequest request)

        public JsonResult GetPlayersForAutoComplete(long gameid)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false; //Need to do this if we return the entire game. If we just get the name; we probably don't.
                var players = db.Player.Where(p => p.GameId == gameid);

                IList<string> playerNames = new List<string>();

                foreach (Player player in players)
                {
                    playerNames.Add(new ProbePlayer(player).PlayerGameName);
                }


                return Json(playerNames, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }

        }//public JsonResult GetPlayersForAutoComplete()

        /*
         * Get all Game Types
         */
        public JsonResult GetSexTypes()
        {
            try
            {
                var itemsVar = EnumHelper.SelectListFor<Person.SexType>();
                var items = itemsVar.ToList();
                items[0].Value = "0";
                items[1].Value = "1";
                items[2].Value = "2";

                return Json(items, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest dsRequest, PlayerDTO playerDTO)
        {

            try
            {
                long gameId = playerDTO.GameId;

                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsGameForLoggedInUser((long)playerDTO.GameId))
                {
                    ModelState.AddModelError("", "Player Update could not be accomplished");
                    return Json(ModelState.ToDataSourceResult());
                }

                if (ModelState.IsValid)
                {
                    Player player = db.Player.Find(playerDTO.Id);
                    player.Sex = playerDTO.Sex;

                    db.Entry(player).State = EntityState.Modified;
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                }

                //return Json(ModelState.ToDataSourceResult());
                return Json(new[] { playerDTO }.ToDataSourceResult(dsRequest, ModelState));


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }
        }//public ActionResult Update([DataSourceRequest] DataSourceRequest dsRequest, PlayerDTO playerDTO)

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Delete([DataSourceRequest] DataSourceRequest request, PlayerDTO playerDTO)
        {
            try
            {
                if (!ProbeValidate.IsGameForLoggedInUser(playerDTO.GameId))
                {
                    ModelState.AddModelError("", "Player Delete could not be accomplished");
                    return Json(ModelState.ToDataSourceResult());
                }


                if (playerDTO != null && ModelState.IsValid)
                {
                    //will delete the game play submissions of the player and then the player.
                    Player player = db.Player.Find(playerDTO.Id);

                    ProbeGame.DeletePlayer(db, player);
                }

                return Json(ModelState.IsValid ? true : ModelState.ToDataSourceResult());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }

        }//public JsonResult Delete([DataSourceRequest] DataSourceRequest request, QuestionDTO questionDTO)

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
