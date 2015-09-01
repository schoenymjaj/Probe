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
using ProbeDAL.Models;
using Probe.Models.View;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Probe.Helpers.Validations;
using Probe.Helpers.GameHelpers;
using Probe.Helpers.Exceptions;
using Probe.Helpers.Mics;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Probe.Controllers
{
    public class GamesController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        // GET: Games
        public ActionResult Index()
        {
            return View();
        }

        // GET: Players
        public ActionResult GameSchedules(long gameid)
        {
            Game game = db.Game.Find(gameid);

            ViewBag.GameId = gameid;
            ViewBag.GameName = game.Name;
            ViewBag.GameTypeName = game.GameType.Name;
            ViewBag.GameEditable = !ProbeValidate.IsGameActive(game);

            return View();
        }

        /*
         * This controller action will support JIT data to the client Grid. the data returned to the
         * client (RAZOR/Kendo MVC Grid wrapper will only see the data that is going to be displayed (one page worths)
         */
        public JsonResult Get([DataSourceRequest]DataSourceRequest request)
        {
            try
            {
                IList<GameDTO> gameDTOList = this.GetAvailableGames();


                return this.Json(gameDTOList.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }

        }//public JsonResult Get([DataSourceRequest]DataSourceRequest request)

        public JsonResult GetGamesForAutoComplete()
        {
            try
            {
                string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

                db.Configuration.LazyLoadingEnabled = false; //Need to do this if we return the entire game. If we just get the name; we probably don't.
                var gameNames = db.Game.Where(g=> g.AspNetUsersId == loggedInUserId).Select(g => g.Name);
                return Json(gameNames, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }

        }//public JsonResult GetGamesForAutoComplete()

        /*
         * Get all Game Types
         */
        public JsonResult GetGameTypes()
        {
            try
            {
                var itemsVar = new SelectList(db.GameType, "Id", "Name");
                var items = itemsVar.ToList();

                return Json(items, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }
        }

        public JsonResult GetGameSchedules([DataSourceRequest]DataSourceRequest request, long gameid)
        {
            try
            {

                Game game = db.Game.Find(gameid);
                ProbeGame probeGame = new ProbeGame(game);
                int nbrQuestions = game.GameQuestions.Count();
                int nbrPlayers = game.Players.Count();


                IList<GameQuestionScheduleDTO> gameQuestionScheduleDTOs = new List<GameQuestionScheduleDTO>();

                DateTime gameLocalStartDate = ClientTimeZoneHelper.ConvertToLocalTime(game.StartDate, false);
                DateTime gameLocalEndDate = ClientTimeZoneHelper.ConvertToLocalTime(game.EndDate, false);

                //override set to true, because this date will be serialized/stringified on the server side
                DateTime gameLocalEndDateforServer = ClientTimeZoneHelper.ConvertToLocalTime(game.EndDate, true);


                string specificStartScheduleDesc = string.Empty;
                switch ((ProbeGameType)game.GameTypeId)
                {
                    case ProbeGameType.Match:
                    case ProbeGameType.Test:
                        if (game.Published && !game.SuspendMode)
                        {
                            specificStartScheduleDesc = "At this time game is active. Players can use their game code to play game and submit their answers up to the game end date (" +
                                gameLocalEndDate.ToString() + "). Game configuration cannot be changed, questions cannot be added or removed, and players cannot be edited or removed." +
                                " There are " + nbrQuestions + " question(s) to be answered for this game." +
                                " There are " + nbrPlayers + " players(s) that have or are playing this game.";
                        }
                        else if (nbrQuestions == 0)
                        {
                            specificStartScheduleDesc = "At this time, game is inactive. The game organizer must add questions to this game from the Question Library (Questions button), set any specifc game configurations (Config button)," +
                                " publish (Publish button), and then distribute the game code to all the players";
                        }
                        else
                        {
                            specificStartScheduleDesc = "At this time, game is inactive. Players cannot find game with a game code nor can they submit their answers." +
                                " There are " + nbrQuestions + " question(s) to be answered for this game." +
                                " There are " + nbrPlayers + " players(s) that have or are playing this game.";
                        }
                        break;
                    case ProbeGameType.LMS:
                        if (game.Published && !game.SuspendMode)
                        {
                            specificStartScheduleDesc = "At this time, game is active. At this time, players can use their game code to play game or submit an answer to the next LMS question up to the game end date (" +
                                gameLocalEndDateforServer.ToString() + "). Game configuration cannot be changed, questions cannot be added or removed, and players cannot be edited or removed." +
                                " There are " + nbrQuestions + "question(s) to be answered for this game." +
                                " There are " + nbrQuestions + "players(s) that have or are playing this game.";
                        }
                        else if (nbrQuestions == 0)
                        {
                            specificStartScheduleDesc = "At this time, game is inactive. The game organizer must add questions to this game from the Question Library (Questions button), set any specifc game configurations (Config button)," +
                                " publish (Publish button), and then distribute the game code to all the players";
                        }
                        else
                        {
                            specificStartScheduleDesc = "Game is inactive. Players cannot find game with the game code nor can they submit an answer to the next LMS question." +
                                " There are " + nbrQuestions + "question(s) to be answered for this game." +
                                " There are " + nbrQuestions + "players(s) that have or are playing this game.";
                        }
                        break;
                }

                GameQuestionScheduleDTO gameQuestionScheduleStartDTO = new GameQuestionScheduleDTO
                {
                    Id = 0,
                    ScheduleName = "Game Start",
                    ScheduleDesc = "Game Start. The game status is " +
                    ((game.Published) ? "published" : "not published") + ((game.SuspendMode) ? "(suspended)" : "") + ". " + specificStartScheduleDesc,
                    StartDate = gameLocalStartDate,
                    InterimDate = gameLocalStartDate,
                    EndDate = gameLocalStartDate
                };
                gameQuestionScheduleDTOs.Add(gameQuestionScheduleStartDTO);

                //For the LMS games, there are individual question schedules
                if (game.GameTypeId == (long)ProbeGameType.LMS)
                {
                    IList<ProbeGameQuestionDeadline> probeGameQuestionDeadlines = probeGame.ProbeGameQuestionDeadlines;
                    foreach (ProbeGameQuestionDeadline pgqd in probeGameQuestionDeadlines)
                    {
                        ChoiceQuestion choiceQuestion = db.ChoiceQuestion.Find(pgqd.QuestionId);

                        GameQuestionScheduleDTO gameQuestionScheduleDTO = new GameQuestionScheduleDTO
                        {
                            Id = 0,
                            ScheduleName = "Question #" + pgqd.QuestionNumber,
                            ScheduleDesc = "Question " + choiceQuestion.Name + ". Text: " + choiceQuestion.Text,
                            //". The start date defines when question will be available to a player to answer." +
                            //" The end date defines when question must be answered by a player." +
                            //" The warning date defines when a warning message is displayed to a player that a question deadline is approaching.",
                            GameId = gameid,
                            QuestionId = pgqd.QuestionId,

                            //StartDate = pgqd.QuestionStartDT,
                            //InterimDate = pgqd.QuestionWarningDT,
                            //EndDate = pgqd.QuestionDeadlineDT,
                            //TimeSpanString = ConvertTimeSpanToString(pgqd.QuestionDeadlineDT.Subtract(pgqd.QuestionStartDT))

                            /* WHEN WE PASS A LIST TO KENDO GRID - IT TAKES CARE OF CONVERTING UTC DATE TO LOCAL */
                            StartDate = ClientTimeZoneHelper.ConvertToLocalTime(pgqd.QuestionStartDT, false),
                            InterimDate = ClientTimeZoneHelper.ConvertToLocalTime(pgqd.QuestionWarningDT, false),
                            EndDate = ClientTimeZoneHelper.ConvertToLocalTime(pgqd.QuestionDeadlineDT, false),
                            TimeSpanString = ConvertTimeSpanToString(
                            ClientTimeZoneHelper.ConvertToLocalTime(pgqd.QuestionDeadlineDT, false).Subtract(ClientTimeZoneHelper.ConvertToLocalTime(pgqd.QuestionStartDT, false)))
                        
                        };
                        gameQuestionScheduleDTOs.Add(gameQuestionScheduleDTO);

                    }//foreach (ProbeGameQuestionDeadline pgqd in probeGameQuestionDeadlines)

                }//if (game.GameTypeId == (long)ProbeGameType.LMS)


                GameQuestionScheduleDTO gameQuestionScheduleEndDTO = new GameQuestionScheduleDTO
                {
                    Id = 0,
                    ScheduleName = "Game End",
                    ScheduleDesc = "Game End. The game will become inactive",
                    StartDate = gameLocalEndDate,
                    InterimDate = gameLocalEndDate,
                    EndDate = gameLocalEndDate
                };
                gameQuestionScheduleDTOs.Add(gameQuestionScheduleEndDTO); 

                return this.Json(gameQuestionScheduleDTOs.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }

        }//public JsonResult GetGameSchedule([DataSourceRequest]DataSourceRequest request, long gameid)

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create([DataSourceRequest] DataSourceRequest request, GameDTO gameDTO)
        {

            try
            {

                ValidateGameCreate(gameDTO);
                if (ModelState.IsValid)
                {

                    //transform DTO to business object (Game)
                    Game game = new Game
                    {
                        Id = 0,
                        AspNetUsersId = User.Identity.GetUserId(),
                        GameTypeId = gameDTO.GameTypeId,
                        Name = gameDTO.Name,
                        Description = gameDTO.Description,
                        Code = gameDTO.Code,

                        //Conversion from local (client-side) to UTC (server-side) is completed in the backend controller
                        StartDate = Probe.Helpers.Mics.ClientTimeZoneHelper.ConvertLocalToUTC(gameDTO.StartDate),
                        EndDate = Probe.Helpers.Mics.ClientTimeZoneHelper.ConvertLocalToUTC(gameDTO.EndDate),

                        //GameUrl = gameDTO.GameUrl, NOT USED
                        Published = false,
                        SuspendMode = gameDTO.SuspendMode,
                        //TestMode = gameDTO.TestMode, NOT USED
                        ClientReportAccess = gameDTO.ClientReportAccess,
                        ACLId = 1 //private
                    };

                    db.Game.Add(game);
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                    //We will send back the game dates that are coverted to UTC
                    gameDTO.StartDate = ClientTimeZoneHelper.ConvertToLocalTime(game.StartDate, false);
                    gameDTO.EndDate = ClientTimeZoneHelper.ConvertToLocalTime(game.EndDate, false);

                    gameDTO.IsActive = ProbeValidate.IsGameActiveOrPlayersExist(game); //updates the IsActive field
                    gameDTO.PlayerCount = 0;
                    gameDTO.PlayerActiveCount = 0;
                    gameDTO.QuestionCount = 0;
                    gameDTO.Id = game.Id; //pass back the new Id to the client

                }

                return Json(new[] { gameDTO }.ToDataSourceResult(request, ModelState));

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.IsValid ? true : ModelState.ToDataSourceResult());
            }

        }//public JsonResult Create([DataSourceRequest] DataSourceRequest request, GameDTO gameDTO)

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest dsRequest, GameDTO gameDTO)
        {

            try
            {

                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsGameForLoggedInUser((long)gameDTO.Id))
                {
                    ModelState.AddModelError("", "Game Update could not be accomplished");
                    return Json(ModelState.ToDataSourceResult());
                }

                ValidateGameEdit(gameDTO);
                if (ModelState.IsValid)
                {
                    Game game = db.Game.Find(gameDTO.Id);

                    //game.AspNetUsersId - THIS IS NOT CHANGING
                    game.GameTypeId = gameDTO.GameTypeId;
                    game.Name = gameDTO.Name;
                    game.Description = gameDTO.Description;
                    game.Code = gameDTO.Code;
                    //game.GameUrl = gameDTO.GameUrl; //NOT USED

                    //Conversion from local (client-side) to UTC (server-side) is completed in the backend controller
                    game.StartDate = Probe.Helpers.Mics.ClientTimeZoneHelper.ConvertLocalToUTC(gameDTO.StartDate);
                    game.EndDate = Probe.Helpers.Mics.ClientTimeZoneHelper.ConvertLocalToUTC(gameDTO.EndDate);

                    game.Published = gameDTO.Published;
                    game.SuspendMode = gameDTO.SuspendMode;
                    //game.TestMode = gameDTO.TestMode; //NOT USED
                    game.ClientReportAccess = gameDTO.ClientReportAccess;
                    //game.ACLId - THIS IS NOT CHANGING

                    db.Entry(game).State = EntityState.Modified;
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                    gameDTO.IsActive = ProbeValidate.IsGameActiveOrPlayersExist(game); //updates the IsActive field

                    //We will send back the game dates that are coverted to UTC
                    gameDTO.StartDate = ClientTimeZoneHelper.ConvertToLocalTime(game.StartDate, false);
                    gameDTO.EndDate = ClientTimeZoneHelper.ConvertToLocalTime(game.EndDate, false);

                }
                

                //return Json(ModelState.ToDataSourceResult());
                return Json(new[] { gameDTO }.ToDataSourceResult(dsRequest, ModelState));


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Json(ModelState.ToDataSourceResult());
            }
        }//public ActionResult Update([DataSourceRequest] DataSourceRequest dsRequest, GameDTO gameDTO)

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Delete([DataSourceRequest] DataSourceRequest request, GameDTO gameDTO)
        {
            try
            {

                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsGameForLoggedInUser((long)gameDTO.Id))
                {
                    ModelState.AddModelError("", "Game Delete could not be accomplished");
                    return Json(ModelState.ToDataSourceResult());
                }

                ValidateGameDelete(gameDTO.Id);
                if (gameDTO != null && ModelState.IsValid)
                {
                    db.Configuration.LazyLoadingEnabled = true;
                    ProbeGame.DeleteGame(this, db, gameDTO.Id);
                }

                return Json(ModelState.IsValid ? true : ModelState.ToDataSourceResult());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }//public JsonResult Delete([DataSourceRequest] DataSourceRequest request, GameDTO gameDTO)

        //MNS - Not being used at the moment 8/28/15
        public JsonResult DeleteStandAlone(long id)
        {
            ResultMessage resultMessage;
            string gameName = db.Game.Find(id).Name;

            try
            {

                //check to ensure the user owns the resources she is trying to access. if not; we get out of here. 
                //Somebody is trying to do bad stuff.
                if (!ProbeValidate.IsGameForLoggedInUser(id))
                {

                    //The message return via an AJAX call
                    resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_UnsuccessfulOperation,
                        MessageType = Helpers.Mics.MessageType.Informational,
                        Message = @"Game Delete could not be accomplished."
                    };

                    return Json(resultMessage, JsonRequestBehavior.AllowGet);
                }// if (!ProbeValidate.IsGameForLoggedInUser(id))


                ValidateGameDelete(id);
                db.Configuration.LazyLoadingEnabled = true;
                ProbeGame.DeleteGame(this, db, id);

                //The message return via an AJAX call
                resultMessage = new ResultMessage
                {
                    MessageId = ProbeConstants.MSG_GameDeleteSuccessful,
                    MessageType = Helpers.Mics.MessageType.Informational,
                    Message = @"The game <span style=""font-style:italic;font-weight:bold"">" +
                    gameName + @"</span> has been deleted successfully."
                };
            }
            catch (GameIsActiveException)
            {
                //The message that the calling RAZOR can use
                resultMessage = new ResultMessage
                {
                    MessageId = ProbeConstants.MSG_GameHasPlayers,
                    MessageType = Helpers.Mics.MessageType.Error,
                    Message = @"The game <span style=""font-style:italic;font-weight:bold"">" +
                    gameName + @"</span> cannot be deleted because it is currently active or has players that have submitted."
                };
            }
            catch (Exception ex)
            {
                //The message return via an AJAX call
                resultMessage = new ResultMessage
                {
                    MessageId = ProbeConstants.MSG_UnsuccessfulOperation,
                    MessageType = Helpers.Mics.MessageType.Error,
                    Message = "The was an error when attempting to delete the game '" +
                    gameName + "'. " + ex.Message
                };
            }


            return Json(resultMessage, JsonRequestBehavior.AllowGet);

        }//public JsonResult Delete([DataSourceRequest] DataSourceRequest request, GameDTO gameDTO)

        public JsonResult Clone(long id)
        {

            try
            {
                //db.Configuration.LazyLoadingEnabled = false; We want lazy loading
                Game clonedGame = ProbeGame.CloneGame(this, db, id);

                NotifyProbe.NotifyGameChanged(User.Identity.Name); //let all clients know where was a game change.

                //The message return via an AJAX call
                ResultMessage resultMessage = new ResultMessage
                {
                    MessageId = ProbeConstants.MSG_GameCloneSuccessful,
                    MessageType = Helpers.Mics.MessageType.Informational,
                    Message = @"The game <span style=""font-style:italic;font-weight:bold"">" +
                    db.Game.Find(id).Name + @"</span> has been cloned successfully to game <span style=""font-style:italic;font-weight:bold"">" +
                    clonedGame.Name + @"</span>"
                };

                return Json(resultMessage, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                //The message return via an AJAX call
                ResultMessage resultMessage = new ResultMessage
                {
                    MessageId = ProbeConstants.MSG_UnsuccessfulOperation,
                    MessageType = Helpers.Mics.MessageType.Error,
                    Message = "The was an error when attempting to clone the game '" +
                    db.Game.Find(id).Name + "'. " + ex.Message
                };

                return Json(resultMessage, JsonRequestBehavior.AllowGet);
            }


        } //public JsonResult Clone(long id)

        public JsonResult Publish(long id, int publishInd)
        {

            ResultMessage resultMessage;

            Game game = db.Game.Find(id);
            if (game == null)
            {
                //The message that the calling RAZOR can use
                resultMessage = new ResultMessage
                {
                    MessageId = ProbeConstants.MSG_GamePublishSuccessful,
                    MessageType = Helpers.Mics.MessageType.Error,
                    Message = (publishInd == 1) ? "The game was not published successfully" : "The game was not unpublished successfully"
                };

                return Json(resultMessage, JsonRequestBehavior.AllowGet);
            }



            if (publishInd == 1)
            {
                try
                {
                    game.Published = true;
                    this.ValidateGamePublish(game);

                    db.Entry(game).State = EntityState.Modified;
                    db.SaveChanges(Request != null ? Request.LogonUserIdentity.Name : null);

                    NotifyProbe.NotifyGameChanged(User.Identity.Name); //let all clients know where was a game change.

                    //The message that the calling RAZOR can use
                    resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GamePublishSuccessful,
                        MessageType = Helpers.Mics.MessageType.Informational,
                        Message = @"The game <span style=""font-style:italic;font-weight:bold"">" +
                                    db.Game.Find(id).Name + @"</span> was published successfully"
                    };
                }
                catch (GameHasNoQuestionsException)
                {
                    //The message that the calling RAZOR can use
                    resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameHasNoQuestions,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A game without questions cannot be published"
                    };
                }
                catch (GameHasPlayersException)
                {
                    //The message that the calling RAZOR can use
                    resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameHasPlayers,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A game with players cannot be published"
                    };
                }
                catch (GameStartGTEndDateException)
                {
                    //The message that the calling RAZOR can use
                    resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameStartGTEndDate,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A game with a start date greater than or equal to it's end date cannot be published"
                    };
                }
                catch (GameEndDateIsPassedException)
                {
                    //The message that the calling RAZOR can use
                    resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameEndDateIsPassed,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A game with an end date that has passed cannot be published"
                    };
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

                    NotifyProbe.NotifyGameChanged(User.Identity.Name); //let all clients know where was a game change.

                    //The message that the calling RAZOR can use
                    resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameUnpublishSuccessful,
                        MessageType = Helpers.Mics.MessageType.Informational,
                        Message = @"The game <span style=""font-style:italic;font-weight:bold"">" +
                                    db.Game.Find(id).Name + @"</span> was unpublished successfully"
                    };
                }
                catch (GameHasPlayersException)
                {
                    //The message that the calling RAZOR can use
                    resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameHasPlayers,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A game with players cannot be unpublished"
                    };
                }
                catch (GameInSuspendModeException)
                {
                    //The message that the calling RAZOR can use
                    resultMessage = new ResultMessage
                    {
                        MessageId = ProbeConstants.MSG_GameInSuspendMode,
                        MessageType = Helpers.Mics.MessageType.Error,
                        Message = "A suspended game cannot be unpublished"
                    };
                }
            }

            return Json(resultMessage, JsonRequestBehavior.AllowGet);

        } //public ActionResult Publish(long id)

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private IList<GameDTO> GetAvailableGames()
        {
            //limit the games to only what the user possesses
            string loggedInUserId = (User.Identity.GetUserId() != null ? User.Identity.GetUserId() : "-1");

            ViewBag.DctGameHasQuestions = ProbeValidate.GetAllGamesDoesHaveQuestions();
            ViewBag.DctAllGamesActiveStatus = ProbeValidate.GetAllGamesStatus();

            IList<GameDTO> gameDTOList = db.Game.Where(g => g.AspNetUsersId == loggedInUserId)
            .Select(g => new GameDTO
            {
                Id = g.Id,
                AspNetUsersId = g.AspNetUsersId,
                GameTypeId = g.GameType.Id,
                Name = g.Name,
                Description = g.Description,
                ACLId = g.ACLId,
                Code = g.Code,
                GameUrl = g.GameUrl,
                StartDate = g.StartDate,
                EndDate = g.EndDate,
                Published = g.Published,
                ClientReportAccess = g.ClientReportAccess,
                PlayerCount = g.Players.Count(),
                PlayerActiveCount = g.Players.Where(p => p.Active).Count(),
                QuestionCount = g.GameQuestions.Count(),
                IsActive = (((DateTime.Compare(DateTime.UtcNow, g.StartDate) > 0 &&
                                DateTime.Compare(DateTime.UtcNow, g.EndDate) <= 0)
                                || (g.Players.Count() > 0))
                                && !g.SuspendMode
                                && g.Published)
            }).ToList();

            /* WHEN WE PASS A LIST TO KENDO GRID - IT TAKES CARE OF CONVERTING UTC DATE TO LOCAL*/
            foreach (GameDTO gameDTO in gameDTOList)
            {
                gameDTO.StartDate = ClientTimeZoneHelper.ConvertToLocalTime(gameDTO.StartDate, false);
                gameDTO.EndDate = ClientTimeZoneHelper.ConvertToLocalTime(gameDTO.EndDate, false);
            }


            return gameDTOList;
        }//private IList<GameDTO> GetAvailableGames()

        private void ValidateGameCreate(GameDTO gameDTO)
        {
            //GamePlay Business Rules
            if (!ProbeValidate.IsCodeValid(gameDTO.Code))
            {
                ModelState.AddModelError("Code", "The code must be at least 5 characters and can only contain letters, numbers, and spaces. It also cannot contain leading or trailing spaces.");
            }
            else if (ProbeValidate.IsCodeExistInProbe(gameDTO.Code))
            {
                ModelState.AddModelError("Code", "The code already exists for In Common.");
            }

            if (ProbeValidate.IsGameNameExistForLoggedInUser(gameDTO.Name))
            {
                ModelState.AddModelError("Name", "The game name already exists for the logged in user.");
            }

            //Game game = new Game();
            //game.StartDate =  Probe.Helpers.Mics.ClientTimeZoneHelper.ConvertLocalToUTC(gameDTO.StartDate);
            //if (ProbeValidate.IsGameStartPassed(game))
            //{
            //    ModelState.AddModelError("", "The game start date has been passed.");
            //}

            Game game = new Game();
            game.StartDate = gameDTO.StartDate;
            game.EndDate = gameDTO.EndDate;
            if (ProbeValidate.IsGameEndDateGreaterThanStartDate(game))
            {
                ModelState.AddModelError("", "The game's start date is passed it's end date.");
            }

        }

        private void ValidateGameEdit(GameDTO gameDTO)
        {
            //GamePlay Business Rules
            if (!ProbeValidate.IsCodeValid(gameDTO.Code))
            {
                ModelState.AddModelError("Code", "The code must be at least 5 characters and can only contain letters, numbers, and spaces. It also cannot contain leading or trailing spaces.");
            }
            else if (ProbeValidate.IsCodeExistInProbe(gameDTO.Id, gameDTO.Code))
            {
                ModelState.AddModelError("Code", "The code already exists for In Common.");
            }

            if (ProbeValidate.IsGameNameExistForLoggedInUser(gameDTO.Id, gameDTO.Name))
            {
                ModelState.AddModelError("Name", "The game name already exists for the logged in user.");
            }

            Game game = new Game();
            game.StartDate = gameDTO.StartDate;
            game.EndDate = gameDTO.EndDate;
            if (ProbeValidate.IsGameEndDateGreaterThanStartDate(game))
            {
                ModelState.AddModelError("", "The game's start date is passed it's end date.");
            }

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
            if (DateTime.Compare(game.EndDate, DateTime.UtcNow) <= 0)
            {
                throw new GameEndDateIsPassedException();
            }

        }//private void ValidateGamePublish(Game game)

        private void ValidateGameDelete(long gameId)
        {

            Game game = db.Game.Find(gameId);
            if (ProbeValidate.IsGameActiveOrPlayersExist(game))
            {
                ModelState.AddModelError("", "A game cannot be deleted that is either active or has a player that has submitted .");
            }

        }//private void ValidateGameDelete(Game game)

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

        private string ConvertTimeSpanToString(TimeSpan timeSpan)
        {
            string timeSpanStr = string.Empty;
            if (timeSpan.Days > 0)
            {
                timeSpanStr = string.Format("{0} Days:{1} Hours:{2} Mins:{3} Secs"
                    , timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }
            else if (timeSpan.Days == 0 && timeSpan.Hours > 0)
            {
                timeSpanStr = string.Format("{0} Hours:{1} Mins:{2} Secs"
                    , timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }
            else if (timeSpan.Days == 0 && timeSpan.Hours == 0 && timeSpan.Minutes > 0)
            {
                timeSpanStr = string.Format("{0} Mins:{1} Secs"
                    , timeSpan.Minutes, timeSpan.Seconds);
            } else if (timeSpan.Days == 0 && timeSpan.Hours == 0 && timeSpan.Minutes == 0)
            {
                timeSpanStr = string.Format("{0} Seconds"
                    ,timeSpan.Seconds);
            }

            return timeSpanStr;
        }

    }
}
