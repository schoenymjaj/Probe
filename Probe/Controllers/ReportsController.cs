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
using System.Web.Script.Serialization;
using Probe.Helpers.Exceptions;
using Probe.Helpers.Validations;

namespace Probe.Controllers
{
    public class ReportsController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        #region PlayerMatch Reporting

        [AllowAnonymous]
        public JsonResult GetGamePlayerMatchMinMaxData(long gameplayid, string code)
        {
            /*
            the gameplayid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameplayid, code);

            var result = db.Database.SqlQuery<GamePlayerMatchMinMaxData>
                                             ("exec GetGamePlayerMatchMinMax " + gameplayid).ToList();

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

        [AllowAnonymous]
        public ActionResult GamePlayerMatchMinMax(long gameplayid, string code, int? mobileind)
        {
            /*
            the gameplayid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameplayid, code);

            ViewBag.GamePlayId = gameplayid;
            ViewBag.GameCode = code;
            ViewBag.GamePlayName = db.GamePlay.Find(gameplayid).Name;
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"]; //get current selected game from GamePlay controller
            ViewBag.NbrQuestions = db.GamePlay.Find(gameplayid).Game.GameQuestions.Count();

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

        [AllowAnonymous]
        public JsonResult GetPlayerMatchSummaryData(long gameplayid, string code, long playerid)
        {
            /*
            the gameplayid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameplayid, code);

            var result = db.Database.SqlQuery<PlayerMatchSummaryData>
                                             ("exec GetPlayerMatchSummary " + gameplayid + "," + playerid).ToList();

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

        [AllowAnonymous]
        public ActionResult PlayerMatchSummary(long gameplayid, string code, long playerid, int? mobileind)
        {
            /*
            the gameplayid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameplayid, code);

            ViewBag.GamePlayId = gameplayid;
            ViewBag.GameCode = code;
            ViewBag.GamePlayName = db.GamePlay.Find(gameplayid).Name;
            ViewBag.PlayerName = db.Player.Find(playerid).FirstName + " - " + db.Player.Find(playerid).NickName;
            ViewBag.PlayerId = playerid;

            ViewBag.NbrQuestions = db.GamePlay.Find(gameplayid).Game.GameQuestions.Count();

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

        [AllowAnonymous]
        public JsonResult GetPlayerMatchDetailData(long gameplayid, string code, long playerid, long matchedplayerid)
        {
            /*
            the gameplayid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameplayid, code);

            int filterType = 0; //get all questions match or no-match

            var result = db.Database.SqlQuery<PlayerMatchDetailData>
                                             ("exec GetPlayerMatchDetail " + gameplayid + "," + playerid + ","
                                             + matchedplayerid + "," + filterType + ",'order by OrderNbr asc'").ToList();


            List<PlayerMatchDetailReturn> reportData = new List<PlayerMatchDetailReturn>();
            foreach (PlayerMatchDetailData row in result)
            {
                PlayerMatchDetailReturn pmdr = new PlayerMatchDetailReturn {
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

        [AllowAnonymous]
        public ActionResult PlayerMatchDetail(long gameplayid, string code, long playerid, long matchedplayerid, int? mobileind)
        {
            /*
            the gameplayid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameplayid, code);

            ViewBag.GamePlayId = gameplayid;
            ViewBag.GameCode = code;
            ViewBag.GamePlayName = db.GamePlay.Find(gameplayid).Name;
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

        #endregion

        #region PlayerTest Reporting

        [AllowAnonymous]
        public JsonResult GetPlayerTestSummaryData(long gameplayid, string code)
        {
            /*
            the gameplayid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameplayid, code);

            var result = db.Database.SqlQuery<PlayerTestSummaryData>
                                             ("exec GetPlayerTestSummary " + gameplayid).ToList();

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

        [AllowAnonymous]
        public ActionResult PlayerTestSummary(long gameplayid, string code, int? mobileind)
        {
            /*
            the gameplayid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameplayid, code);

            ViewBag.GamePlayId = gameplayid;
            ViewBag.GameCode = code;
            ViewBag.GamePlayName = db.GamePlay.Find(gameplayid).Name;
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"];
            ViewBag.NbrQuestions = db.GamePlay.Find(gameplayid).Game.GameQuestions.Count();

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

        [AllowAnonymous]
        public JsonResult GetPlayerTestDetailData(long gameplayid, string code, long playerid)
        {
            /*
            the gameplayid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameplayid, code);

            var result = db.Database.SqlQuery<PlayerTestDetailData>
                                             ("exec GetPlayerTestDetail " + gameplayid + "," + playerid).ToList();


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

        [AllowAnonymous]
        public ActionResult PlayerTestDetail(long gameplayid, string code, long playerid, int? mobileind)
        {
            /*
            the gameplayid and code passed must correlate or they may be something malicious going on. so we stop 
            the response ASAP and throw an exception AND WE DO NOT CATCH IT, which should be picked up by Elmah. Exception handling here
            have to be improved big-time
            */
            ProbeValidate.ValidateGameCodeVersusId(gameplayid, code);

            ViewBag.GamePlayId = gameplayid;
            ViewBag.GameCode = code;
            ViewBag.GamePlayName = db.GamePlay.Find(gameplayid).Name;
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

        #endregion

    }

}