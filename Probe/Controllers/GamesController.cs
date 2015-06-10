using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Probe.DAL;
using Probe.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Probe.Helpers.Validations;
using Probe.Helpers.GameHelpers;
using Probe.Helpers.Exceptions;
using Probe.Helpers.Mics;

namespace Probe.Controllers
{
    public class GamesController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: Games
        public ActionResult Index()
        {
            IList<Game> gameList = this.GetAvailableGames();

            if (Session["ResultMessage"] != null)
            {
                ViewBag.ResultMessage = (ResultMessage)Session["ResultMessage"];
                Session["ResultMessage"] = null;
            }

            return View(gameList);
        }

        // GET: Games/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Game game = db.Game.Find(id);

            //debug
            //ProbeGame.UpdateAllGamePlayerStatuses();

            if (game == null)
            {
                return HttpNotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public ActionResult Create()
        {
            ViewBag.GameTypeId = new SelectList(db.GameType, "Id", "Name");

            Game game = new Game();
            if (User.Identity.GetUserId() != null)
            {
                game.AspNetUsersId = User.Identity.GetUserId();
                game.StartDate = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, game.StartDate.Second));
                game.EndDate = game.StartDate.AddYears(1);
            }

            return View(game);
        }

        // POST: Games/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GameTypeId,Name,Description,Code,GameUrl,StartDate,EndDate,SuspendMode,TestMode,ClientReportAccess,AspNetUsersId")] Game game)
        {

            ValidateGameCreate(game);
            if (ModelState.IsValid)
            {
                //covert dates to UTC for the database
                game.StartDate = game.StartDate.ToUniversalTime();
                game.EndDate = game.EndDate.ToUniversalTime();

                game.Published = false; ////when creating a game, it will initially not published
                game.SuspendMode = false; 
                game.ACLId = 1; //Private 
                db.Game.Add(game);
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                return RedirectToAction("Index");
            }

            ViewBag.GameTypeId = new SelectList(db.GameType, "Id", "Name", game.GameTypeId);
            return View(game);
        }

        // GET: Games/Clone
        public ActionResult Clone(long id)
        {
            //limit the questions to only what the user possesses
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            //db.Configuration.LazyLoadingEnabled = false; We want lazy loading
            Game clonedGame = ProbeGame.CloneGame(this, db, id);

            //The message that the calling RAZOR can use
            ResultMessage resultMessage = new ResultMessage
            {
                MessageId = ProbeConstants.MSG_GameCloneSuccessful,
                MessageType = Helpers.Mics.MessageType.Informational,
                Message = "The game '" +
                db.Game.Find(id).Name + "' has been cloned successfully to game '" +
                clonedGame.Name + "'"
            };
            Session["ResultMessage"] = resultMessage;

            return RedirectToAction("Index");

        } //public ActionResult Clone(long id)

        // GET: Games/Publish
        public ActionResult Publish(long id, int publishInd)
        {

            Game game = db.Game.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }


            if (publishInd == 1)
            {
                try
                {
                    game.Published = true;
                    this.ValidateGamePublish(game);

                    db.Entry(game).State = EntityState.Modified;
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                    //The message that the calling RAZOR can use
                    ResultMessage resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GamePublishSuccessful,
                        MessageType = Helpers.Mics.MessageType.Informational,
                        Message = "The game was published successfully"
                    };
                    Session["ResultMessage"] = resultMessage;
                }
                catch (GameHasNoQuestionsException)
                {
                    //The message that the calling RAZOR can use
                    ResultMessage resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameHasNoQuestions,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A game without questions cannot be published"
                    };
                    Session["ResultMessage"] = resultMessage;
                }
                catch (GameHasPlayersException)
                {
                    //The message that the calling RAZOR can use
                    ResultMessage resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameHasPlayers,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A game with players cannot be published"
                    };
                    Session["ResultMessage"] = resultMessage;
                }
                catch (GameStartGTEndDateException)
                {
                    //The message that the calling RAZOR can use
                    ResultMessage resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameStartGTEndDate,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A game with a start date greater than or equal to it's end date cannot be published"
                    };
                    Session["ResultMessage"] = resultMessage;
                }
                catch (GameEndDateIsPassedException)
                {
                    //The message that the calling RAZOR can use
                    ResultMessage resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameEndDateIsPassed,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A game with an end date that has passed cannot be published"
                    };
                    Session["ResultMessage"] = resultMessage;
                }
            }
            else
            {
                try
                {
                    game.Published = false;
                    this.ValidateGameUnpublish(game);

                    db.Entry(game).State = EntityState.Modified;
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                    //The message that the calling RAZOR can use
                    ResultMessage resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameUnpublishSuccessful,
                        MessageType = Helpers.Mics.MessageType.Informational,
                        Message = "The game was unpublished successfully"
                    };
                    Session["ResultMessage"] = resultMessage;
                }
                catch (GameHasPlayersException)
                {
                    //The message that the calling RAZOR can use
                    ResultMessage resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameHasPlayers,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A game with players cannot be unpublished"
                    };
                    Session["ResultMessage"] = resultMessage;
                }
                catch (GameInSuspendModeException)
                {
                    //The message that the calling RAZOR can use
                    ResultMessage resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameInSuspendMode,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A suspended game cannot be unpublished"
                    };
                    Session["ResultMessage"] = resultMessage;
                }
            }

            return RedirectToAction("Index");

        } //public ActionResult Publish(long id)

        // GET: Games/Edit/5
        public ActionResult Edit(long? id)
        {
            ViewBag.DctAllGamesActiveStatus = ProbeValidate.GetAllGamesStatus();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Game game = db.Game.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            ViewBag.GameTypeId = new SelectList(db.GameType, "Id", "Name", game.GameTypeId);

            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        ////[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GameTypeId,Name,Description,Code,GameUrl,StartDate,EndDate,Published,SuspendMode,TestMode,ClientReportAccess,AspNetUsersId,ACLId")] Game game)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameForLoggedInUser(game.Id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //remember the local version of the start and end dates for later
            DateTime localStartDate = game.StartDate;
            DateTime localEndDate = game.EndDate;

            ValidateGameEdit(game);
            if (ModelState.IsValid)
            {
                //convert the dates that are in local time of client to UTC for storage in the database
                game.StartDate = ClientTimeZoneHelper.ConvertLocalToUTC(game.StartDate);
                game.EndDate = ClientTimeZoneHelper.ConvertLocalToUTC(game.EndDate);

                db.Entry(game).State = EntityState.Modified;
                db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);
                return RedirectToAction("Index");
            }
            ViewBag.GameTypeId = new SelectList(db.GameType, "Id", "Name", game.GameTypeId);

            ViewBag.DctAllGamesActiveStatus = ProbeValidate.GetAllGamesStatus();

            //all of this is a hack; in order to properly display the game name in the edit screen when there is a validation error
            var gameUpdate = db.Game.Find(game.Id);
            gameUpdate.Id = game.Id;
            gameUpdate.GameTypeId = game.GameTypeId;
            gameUpdate.Name = game.Name;
            gameUpdate.Description = game.Description;
            gameUpdate.Code = game.Code;
            gameUpdate.GameUrl = game.GameUrl;
            gameUpdate.StartDate = localStartDate;
            gameUpdate.EndDate = localEndDate;
            gameUpdate.SuspendMode = game.SuspendMode;
            gameUpdate.TestMode = game.TestMode;
            gameUpdate.ClientReportAccess = game.ClientReportAccess;
            return View(gameUpdate);
        }

        // GET: Games/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameForLoggedInUser((long)id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Game game = db.Game.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
            //Somebody is trying to do bad stuff.
            if (!ProbeValidate.IsGameForLoggedInUser(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Configuration.LazyLoadingEnabled = true;
            ProbeGame.DeleteGame(this, db, id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private IList<Game> GetAvailableGames()
        {
            //limit the games to only what the user possesses
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            ViewBag.DctGameHasQuestions = ProbeValidate.GetAllGamesDoesHaveQuestions();
            ViewBag.DctAllGamesActiveStatus = ProbeValidate.GetAllGamesStatus();

            var gameList = db.Game.Where(g => g.AspNetUsersId == loggedInUserId).Include(g => g.GameType).ToList();

            return gameList;
        }//private IList<Game> GetAvailableGames()

        private void ValidateGameCreate(Game game)
        {
            //GamePlay Business Rules
            if (ProbeValidate.IsCodeExistInProbe(game.Code))
            {
                ModelState.AddModelError("Code", "The code already exists for In Common.");
            }
            else if (!ProbeValidate.IsCodeValid(game.Code))
            {
                ModelState.AddModelError("Code", "The code can only contain letters, numbers, and spaces. It also cannot contain leading or trailing spaces.");
            }
            if (ProbeValidate.IsGameNameExistForLoggedInUser(game.Name))
            {
                ModelState.AddModelError("Name", "The game name already exists for the logged in user.");
            }

            //if (!ProbeValidate.DoesGameHaveQuestions(game.Id))
            //{
            //    ModelState.AddModelError("GameId", "The game must have at least one question.");
            //}

        }

        private void ValidateGameEdit(Game game)
        {
            //GamePlay Business Rules
            if (ProbeValidate.IsCodeExistInProbe(game.Id, game.Code))
            {
                ModelState.AddModelError("Code", "The code already exists for In Common.");
            }
            else if (!ProbeValidate.IsCodeValid(game.Code))
            {
                ModelState.AddModelError("Code", "The code can only contain letters, numbers, and spaces. It also cannot contain leading or trailing spaces.");
            }

            if (ProbeValidate.IsGameNameExistForLoggedInUser(game.Id, game.Name))
            {
                ModelState.AddModelError("Name", "The game name already exists for the logged in user.");
            }

            //if (!ProbeValidate.DoesGameHaveQuestions(game.Id))
            //{
            //    ModelState.AddModelError("GameId", "The game for a game play must have at least one question.");
            //}

        }//private void ValidateGameEdit(Game game)

        private void ValidateGamePublish(Game game)
        {

            //A game must have questions to publish
            if (db.GameQuestion.Where(gq => gq.GameId == game.Id).Count() == 0)
            {
                throw new GameHasNoQuestionsException();
            }

            //A game cannot have players to publish
            if (db.Player.Where(p => p.GameId == game.Id).Count() > 0)
            {
                throw new GameHasPlayersException();
            }

            //A game cannot have end date less than start date to publish
            if (DateTime.Compare(game.StartDate, game.EndDate) > 0)
            {
                throw new GameStartGTEndDateException();
            }

            //A game cannot have an end date in the passed to publish
            if (DateTime.Compare(game.EndDate, DateTime.UtcNow ) <= 0)
            {
                throw new GameEndDateIsPassedException();
            }

        }//private void ValidateGamePublish(Game game)

        private void ValidateGameUnpublish(Game game)
        {

            //A game cannot have players to unpublish
            if (db.Player.Where(p => p.GameId == game.Id).Count() > 0)
            {
                throw new GameHasPlayersException();
            }

            //A game must be in suspend mode to unpublish
            if (game.SuspendMode)
            {
                throw new GameInSuspendModeException();
            }

        }//private void ValidateGameUnpublish(Game game)

    }
}
