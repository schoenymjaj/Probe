using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Probe.DAL;
using Probe.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Probe.Helpers.Validations;

namespace Probe.Controllers
{
    public class GameConfigurationsController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? SelectedGame)
        {
            //limit the games to only what the user possesses
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            /*
             * If SelectedGame(passed in) has a value then we get the game name only
             * If SelectedGame(passed in) does not have a value; then we get the first game we find for the user.
             * if the user doesn't have any games yet then we let the SelectedGame still set to null
             */
            string gameName = string.Empty;
            if (SelectedGame != null)
            {
                gameName = db.Game.Find(SelectedGame).Name;
            }
            else
            {
                Game game = db.Game.Where(g => g.AspNetUsersId == loggedInUserId).OrderBy(g => g.Name).FirstOrDefault();
                SelectedGame = (int)game.Id;
                gameName = game.Name;
            }


            Session["CurrentSelectedGame"] = SelectedGame;
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"];
            ViewBag.DctGameActive = ProbeValidate.GetAllGamesStatus();


            var games = db.Game.Where(g => g.AspNetUsersId == loggedInUserId).OrderBy(g => g.Name).ToList();
            ViewBag.SelectedGame = new SelectList(games, "Id", "Name", SelectedGame);
            int gameId = SelectedGame.GetValueOrDefault();

            /*
                * Outer join with configurationG and gameconfiguration table records
                */
            var gameConfigurations = db.ConfigurationG
                .Where(c => c.ConfigurationType != ConfigurationG.ProbeConfigurationType.GLOBAL)
                .SelectMany(c => c.GameConfigurations.Where(gc => loggedInUserId != "-1" && gc.Game.Id == gameId).DefaultIfEmpty(),
                (c, gc) =>
                    new GameConfigurationDTO
                    {
                        Id = (gc.Id != null) ? gc.Id : -1,
                        ConfigurationGId = c.Id,
                        GameId = gameId,
                        GameName = gameName,
                        Name = c.Name,
                        Description = c.Description,
                        DataTypeG = c.DataTypeG,
                        ConfigurationType = c.ConfigurationType,
                        Value = (gc.Value != null) ? gc.Value : c.Value
                    }
                ).OrderBy(gcX => gcX.Name);

            return View(gameConfigurations.ToList());

        }

        // GET: GameConfigurations/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int gameId, int configurationGid)
        {
            Session["CurrentSelectedGame"] = gameId;
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"];

            string gcName = db.Game.Find(gameId).Name;

            GameConfigurationDTO gameConfigurationDTO = db.ConfigurationG
                .Where(c => c.ConfigurationType == ConfigurationG.ProbeConfigurationType.GAME && c.Id == configurationGid)
                .SelectMany(c => c.GameConfigurations.Where(gc => gc.Game.Id == gameId).DefaultIfEmpty(),
                (c, gc) =>
                    new GameConfigurationDTO
                    {
                        Id = (gc.Id != null) ? gc.Id : -1,
                        ConfigurationGId = c.Id,
                        GameId = gameId,
                        GameName = gcName,
                        Name = c.Name,
                        Description = c.Description,
                        DataTypeG = c.DataTypeG,
                        ConfigurationType = c.ConfigurationType,
                        Value = (gc.Value != null) ? gc.Value : c.Value
                    }
                ).First();


            if (gameConfigurationDTO == null)
            {
                return HttpNotFound();
            }
            return View(gameConfigurationDTO);
        }

        // GET: GameConfigurations/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create(int? SelectedGame)
        {
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", SelectedGame);
            return View();
        }

        // POST: GameConfigurations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GameId,Name,Description,Value")] GameConfiguration gameConfiguration)
        {
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId);

            if (ModelState.IsValid)
            {
                db.GameConfiguration.Add(gameConfiguration);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index", new { SelectedGame = ViewBag.GameId.SelectedValue });
            }

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId);
            return View(gameConfiguration);
        }

        // GET: GameConfigurations/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int gameId, int configurationGid)
        {

            Session["CurrentSelectedGame"] = gameId;
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"];

            string gcName = db.Game.Find(gameId).Name;

            GameConfigurationDTO gameConfigurationDTO = db.ConfigurationG
                .Where(c => c.ConfigurationType == ConfigurationG.ProbeConfigurationType.GAME && c.Id == configurationGid)
                .SelectMany(c => c.GameConfigurations.Where(gc => gc.Game.Id == gameId).DefaultIfEmpty(),
                (c, gc) =>
                    new GameConfigurationDTO
                    {
                        Id = (gc.Id != null) ? gc.Id : -1,
                        ConfigurationGId = c.Id,
                        GameId = gameId,
                        GameName = gcName,
                        Name = c.Name,
                        Description = c.Description,
                        DataTypeG = c.DataTypeG,
                        ConfigurationType = c.ConfigurationType,
                        Value = (gc.Value != null) ? gc.Value : c.Value
                    }
                ).First();


            if (gameConfigurationDTO == null)
            {
                return HttpNotFound();
            }
            return View(gameConfigurationDTO);
        }

        // POST: GameConfigurations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ConfigurationGId,GameId,Name,Description,Value")] GameConfigurationDTO gameConfigurationDTO)
        {
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
                return RedirectToAction("Index", new { SelectedGame = gameConfigurationDTO.GameId });
            }
            else
            {
                //to edit an existing record; you need to seed the id with the existing PK
                gameConfiguration.Id = (int)gameConfigurationDTO.Id;

                db.Entry(gameConfiguration).State = EntityState.Modified;
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index", new { SelectedGame = gameConfigurationDTO.GameId });
            }

        }

        // GET: GameConfigurations/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GameConfiguration gameConfiguration = db.GameConfiguration.Find(id);
            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId); //persist the selected game

            if (gameConfiguration == null)
            {
                return HttpNotFound();
            }
            return View(gameConfiguration);
        }

        // POST: GameConfigurations/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            GameConfiguration gameConfiguration = db.GameConfiguration.Find(id);

            ViewBag.GameId = new SelectList(db.Game, "Id", "Name", gameConfiguration.GameId); //persist the selected game

            db.GameConfiguration.Remove(gameConfiguration);
            db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
            return RedirectToAction("Index", new { SelectedGame = ViewBag.GameId.SelectedValue });
        }

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
