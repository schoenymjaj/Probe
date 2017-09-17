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
using Probe.Models.API;
using System.Web.Script.Serialization;
using Probe.Helpers.Exceptions;
using Probe.Helpers.Validations;
using Probe.Helpers.Mics;

namespace Probe.Controllers
{
    public class ReportsController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        #region PlayerMatch Reporting

        [AllowAnonymous]
        public JsonResult GetGamePlayerMatchMinMaxData(long gameid, string code)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception
                */
                ProbeValidate.ValidateGameCodeVersusId(gameid, code);

                var result = db.Database.SqlQuery<GamePlayerMatchMinMaxData>
                                                 ("exec GetGamePlayerMatchMinMax " + gameid).ToList();

                List<GamePlayerMatchMinMaxReturn> reportData = new List<GamePlayerMatchMinMaxReturn>();
                foreach (GamePlayerMatchMinMaxData row in result)
                {

                    GamePlayerMatchMinMaxReturn gpmmmr = new GamePlayerMatchMinMaxReturn
                    {
                        PlayerId = row.PlayerId,
                        PlayerName = row.PlayerName,
                        MaleMaxNbrOfMatched = row.MaleMaxNbrOfMatched,
                        MaleMaxMatchedPlayerId = row.MaleMaxMatchedPlayerId,
                        MaleMaxPlayerName = row.MaleMaxPlayerName,
                        MaleMinNbrOfMatched = row.MaleMinNbrOfMatched,
                        MaleMinMatchedPlayerId = row.MaleMinMatchedPlayerId,
                        MaleMinPlayerName = row.MaleMinPlayerName,
                        FemaleMaxNbrOfMatched = row.FemaleMaxNbrOfMatched,
                        FemaleMaxMatchedPlayerId = row.FemaleMaxMatchedPlayerId,
                        FemaleMaxPlayerName = row.FemaleMaxPlayerName,
                        FemaleMinNbrOfMatched = row.FemaleMinNbrOfMatched,
                        FemaleMinMatchedPlayerId = row.FemaleMinMatchedPlayerId,
                        FemaleMinPlayerName = row.FemaleMinPlayerName,


                    };
                    reportData.Add(gpmmmr);

                }

                return Json(reportData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }
        }//public JsonResult GetGamePlayerMatchMinMaxData(long gameid, string code)

        [AllowAnonymous]
        public ActionResult GamePlayerMatchMinMax(long gameid, string code, int? mobileind)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception 
                */
                ProbeValidate.ValidateGameCodeVersusId(gameid, code);

                ViewBag.GameId = gameid;
                ViewBag.GameCode = code;
                ViewBag.GameName = db.Game.Find(gameid).Name;
                ViewBag.NbrQuestions = db.Game.Find(gameid).GameQuestions.Count();

                if (mobileind == null || mobileind == 0)
                {
                    ViewBag.MobileInd = false;
                    ViewBag.CordovaInd = false;
                    return View();
                }
                else
                {
                    ViewBag.MobileInd = true;
                    if (mobileind == 2)
                    {
                        ViewBag.CordovaInd = true;
                    }
                    else
                    {
                        ViewBag.CordovaInd = false;
                    }
                    return View("GamePlayerMatchMinMax", "_MobileLayout");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }

        }//public ActionResult GamePlayerMatchMinMax(long gameid, string code, int? mobileind)

        [AllowAnonymous]
        public JsonResult GetPlayerMatchSummaryData(long gameid, string code, long playerid)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception
                */
                ProbeValidate.ValidateGameCodeVersusId(gameid, code);

                var result = db.Database.SqlQuery<PlayerMatchSummaryData>
                                                 ("exec GetPlayerMatchSummary " + gameid + "," + playerid).ToList();

                List<PlayerMatchSummaryReturn> reportData = new List<PlayerMatchSummaryReturn>();
                foreach (PlayerMatchSummaryData row in result)
                {

                    PlayerMatchSummaryReturn pmsr = new PlayerMatchSummaryReturn
                                                    {
                                                        Id = row.MatchedPlayerId,
                                                        Name = row.MatchedPlayerName,
                                                        Value = row.NbrOfMatched
                                                    };
                    reportData.Add(pmsr);

                }

                return Json(reportData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }

        }//public JsonResult GetPlayerMatchSummaryData(long gameid, string code, long playerid)

        [AllowAnonymous]
        public ActionResult PlayerMatchSummary(long gameid, string code, long playerid, int? mobileind)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception 
                */
                ProbeValidate.ValidateGameCodeVersusId(gameid, code);

                ViewBag.GameId = gameid;
                ViewBag.GameCode = code;
                ViewBag.GameName = db.Game.Find(gameid).Name;
                ViewBag.PlayerName = db.Player.Find(playerid).FirstName + " - " + db.Player.Find(playerid).NickName;
                ViewBag.PlayerId = playerid;

                ViewBag.NbrQuestions = db.Game.Find(gameid).GameQuestions.Count();

                if (mobileind == null || mobileind == 0)
                {
                    ViewBag.MobileInd = false;
                    ViewBag.CordovaInd = false;
                    return View();
                }
                else
                {
                    ViewBag.MobileInd = true;
                    if (mobileind == 2)
                    {
                        ViewBag.CordovaInd = true;
                    }
                    else
                    {
                        ViewBag.CordovaInd = false;
                    }
                    return View("PlayerMatchSummary", "_MobileLayout");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }

        }//public ActionResult PlayerMatchSummary(long gameid, string code, long playerid, int? mobileind)

        [AllowAnonymous]
        public JsonResult GetPlayerMatchDetailData(long gameId, string code, long playerid, long matchedplayerid)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception
                */
                ProbeValidate.ValidateGameCodeVersusId(gameId, code);

                int filterType = 0; //get all questions match or no-match

                var result = db.Database.SqlQuery<PlayerMatchDetailData>
                                                 ("exec GetPlayerMatchDetail " + gameId + "," + playerid + ","
                                                 + matchedplayerid + "," + filterType + ",'order by OrderNbr asc'").ToList();


                List<PlayerMatchDetailReturn> reportData = new List<PlayerMatchDetailReturn>();
                foreach (PlayerMatchDetailData row in result)
                {
                    PlayerMatchDetailReturn pmdr = new PlayerMatchDetailReturn
                    {
                        PlayerName = row.PlayerName,
                        MatchedPlayerId = row.MatchedPlayerId,
                        MatchedPlayerName = row.MatchedPlayerName,
                        QuestionId = row.QuestionId,
                        Question = row.Question,
                        PlayerChoice = row.PlayerChoice,
                        MatchedPlayerChoice = row.MatchedPlayerChoice,
                        Match = row.Match,
                        PercentChosen = row.PercentChosen
                    };

                    reportData.Add(pmdr);
                }

                return Json(reportData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }

        }//public JsonResult GetPlayerMatchDetailData(long gameId, string code, long playerid, long matchedplayerid)

        [AllowAnonymous]
        public ActionResult PlayerMatchDetail(long gameid, string code, long playerid, long matchedplayerid, int? mobileind)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception 
                */
                ProbeValidate.ValidateGameCodeVersusId(gameid, code);

                ViewBag.GameId = gameid;
                ViewBag.GameCode = code;
                ViewBag.GameName = db.Game.Find(gameid).Name;
                ViewBag.PlayerId = playerid;
                ViewBag.MatchedPlayerId = matchedplayerid;

                if (mobileind == null || mobileind == 0)
                {
                    ViewBag.MobileInd = false;
                    ViewBag.CordovaInd = false;
                    return View();
                }
                else
                {
                    ViewBag.MobileInd = true;
                    if (mobileind == 2)
                    {
                        ViewBag.CordovaInd = true;
                    }
                    else
                    {
                        ViewBag.CordovaInd = false;
                    }
                    return View("PlayerMatchDetail", "_MobileLayout");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }

        }//public ActionResult PlayerMatchDetail(long gameid, string code, long playerid, long matchedplayerid, int? mobileind)

        #endregion

        #region PlayerTest Reporting

        [AllowAnonymous]
        public JsonResult GetPlayerTestSummaryData(long gameid, string code)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception
                */
                ProbeValidate.ValidateGameCodeVersusId(gameid, code);

                var result = db.Database.SqlQuery<PlayerTestSummaryData>
                                                 ("exec GetPlayerTestSummary " + gameid).ToList();

                List<PlayerTestSummaryReturn> reportData = new List<PlayerTestSummaryReturn>();
                foreach (PlayerTestSummaryData row in result)
                {

                    PlayerTestSummaryReturn ptsr = new PlayerTestSummaryReturn
                                                    {
                                                        Id = row.PlayerId,
                                                        Name = row.PlayerName,
                                                        Value = row.PercentCorrect
                                                    };
                    reportData.Add(ptsr);

                }

                return Json(reportData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }

        }//public JsonResult GetPlayerTestSummaryData(long gameid, string code)

        [AllowAnonymous]
        public ActionResult PlayerTestSummary(long gameid, string code, int? mobileind)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception
                */
                ProbeValidate.ValidateGameCodeVersusId(gameid, code);

                ViewBag.GameId = gameid;
                ViewBag.GameCode = code;
                ViewBag.GameName = db.Game.Find(gameid).Name;
                ViewBag.NbrQuestions = db.Game.Find(gameid).GameQuestions.Count();

                if (mobileind == null || mobileind == 0)
                {
                    ViewBag.MobileInd = false;
                    ViewBag.CordovaInd = false;
                    return View();
                }
                else
                {
                    ViewBag.MobileInd = true;
                    if (mobileind == 2)
                    {
                        ViewBag.CordovaInd = true;
                    }
                    else
                    {
                        ViewBag.CordovaInd = false;
                    }
                    return View("PlayerTestSummary", "_MobileLayout");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }


        }//public ActionResult PlayerTestSummary(long gameid, string code, int? mobileind)

        [AllowAnonymous]
        public JsonResult GetPlayerTestDetailData(long gameId, string code, long playerid)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception
                */
                ProbeValidate.ValidateGameCodeVersusId(gameId, code);

                var result = db.Database.SqlQuery<PlayerTestDetailData>
                                                 ("exec GetPlayerTestDetail " + gameId + "," + playerid).ToList();


                List<PlayerTestDetailReturn> reportData = new List<PlayerTestDetailReturn>();
                foreach (PlayerTestDetailData row in result)
                {
                    PlayerTestDetailReturn ptdr = new PlayerTestDetailReturn
                    {
                        PlayerName = row.PlayerName,
                        QuestionId = row.QuestionId,
                        Question = row.Question,
                        OrderNbr = row.OrderNbr,
                        SelectedChoices = row.SelectedChoices,
                        CorrectChoices = row.CorrectChoices,
                        QuestionCorrect = row.QuestionCorrect,
                        PercentCorrect = row.PercentCorrect
                    };

                    reportData.Add(ptdr);
                }

                return Json(reportData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }

        }//public JsonResult GetPlayerTestDetailData(long gameId, string code, long playerid)

        [AllowAnonymous]
        public ActionResult PlayerTestDetail(long gameid, string code, long playerid, int? mobileind)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception 
                */
                ProbeValidate.ValidateGameCodeVersusId(gameid, code);

                ViewBag.GameId = gameid;
                ViewBag.GameCode = code;
                ViewBag.GameName = db.Game.Find(gameid).Name;
                ViewBag.PlayerId = playerid;

                //check mobile indicator from request
                if (mobileind == null || mobileind == 0)
                {
                    ViewBag.MobileInd = false;
                    ViewBag.CordovaInd = false;
                    return View();
                }
                else
                {
                    ViewBag.MobileInd = true;
                    if (mobileind == 2)
                    {
                        ViewBag.CordovaInd = true;
                    }
                    else
                    {
                        ViewBag.CordovaInd = false;
                    }
                    return View("PlayerTestDetail", "_MobileLayout");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }

        }//public ActionResult PlayerTestDetail(long gameid, string code, long playerid, int? mobileind)

        #endregion

        #region PlayerLMS Reporting

        [AllowAnonymous]
        public ActionResult GameLMSSummary(long gameid, string code, int? mobileind)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception
                */
                ProbeValidate.ValidateGameCodeVersusId(gameid, code);

                ViewBag.GameId = gameid;
                ViewBag.GameCode = code;
                ViewBag.GameName = db.Game.Find(gameid).Name;

                if (mobileind == null || mobileind == 0)
                {
                    ViewBag.MobileInd = false;
                    ViewBag.CordovaInd = false;
                    return View();
                }
                else
                {
                    ViewBag.MobileInd = true;
                    if (mobileind == 2)
                    {
                        ViewBag.CordovaInd = true;
                    }
                    else
                    {
                        ViewBag.CordovaInd = false;
                    }
                    return View("GameLMSSummary", "_MobileLayout");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }

        }//public ActionResult GameLMSSummary(long gameid, string code, int? mobileind)

        [AllowAnonymous]
        public ActionResult PlayerLMSSummary(long gameid, string code, int playerstatusfilter, int? mobileind)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception
                */
                ProbeValidate.ValidateGameCodeVersusId(gameid, code);

                ViewBag.GameId = gameid;
                ViewBag.GameCode = code;
                ViewBag.PlayerStatusFilter = playerstatusfilter;
                ViewBag.GameName = db.Game.Find(gameid).Name;

                if (mobileind == null || mobileind == 0)
                {
                    ViewBag.MobileInd = false;
                    ViewBag.CordovaInd = false;
                    return View();
                }
                else
                {
                    ViewBag.MobileInd = true;
                    if (mobileind == 2)
                    {
                        ViewBag.CordovaInd = true;
                    }
                    else
                    {
                        ViewBag.CordovaInd = false;
                    }
                    return View("PlayerLMSSummary", "_MobileLayout");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }

        }//public ActionResult PlayerLMSSummary(long gameid, string code, int playerstatusfilter, int? mobileind)

        [AllowAnonymous]
        public ActionResult PlayerLMSDetail(long gameid, string code, long playerid, int? mobileind)
        {
            try
            {
                /*
                the gameId and code passed must correlate or they may be something malicious going on. so we stop 
                the response ASAP and throw an exception
                */
                ProbeValidate.ValidateGameCodeVersusId(gameid, code);

                ViewBag.GameId = gameid;
                ViewBag.GameCode = code;
                ViewBag.PlayerId = playerid;
                ViewBag.GameName = db.Game.Find(gameid).Name;

                if (mobileind == null || mobileind == 0)
                {
                    ViewBag.MobileInd = false;
                    ViewBag.CordovaInd = false;
                    return View();
                }
                else
                {
                    ViewBag.MobileInd = true;
                    if (mobileind == 2)
                    {
                        ViewBag.CordovaInd = true;
                    }
                    else
                    {
                        ViewBag.CordovaInd = false;
                    }
                    return View("PlayerLMSDetail", "_MobileLayout");
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex); //log to elmah
                ModelState.AddModelError("", ProbeConstants.MSG_UnsuccessfulOperation_STR);
                return Json(ModelState);
            }

        }//public ActionResult PlayerLMSDetail(long gameid, string code, long playerid, int? mobileind)

        #endregion

    }

}