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

namespace Probe.Controllers
{
    public class ReportsController : Controller
    {
        private ProbeDataContext db = new ProbeDataContext();

        #region JSON Data Structures

        public class GamePlayerDemos
        {
            public long GamePlayId { get; set; }
            public string GamePlayName { get; set; }
            public string GameType { get; set; }
            public long PlayerId { get; set; }
            public string PlayerName { get; set; }
        }

        #region PlayerMatch JSON Data Structures

        public class GamePlayerMatchMinMaxData : GamePlayerDemos
        {
            public int? MaleMaxNbrOfMatched { get; set; }
            public long? MaleMaxMatchedPlayerId { get; set; }
            public string MaleMaxPlayerName { get; set; }
            public int? MaleMinNbrOfMatched { get; set; }
            public long? MaleMinMatchedPlayerId { get; set; }
            public string MaleMinPlayerName { get; set; }
            public int? FemaleMaxNbrOfMatched { get; set; }
            public long? FemaleMaxMatchedPlayerId { get; set; }
            public string FemaleMaxPlayerName { get; set; }
            public int? FemaleMinNbrOfMatched { get; set; }
            public long? FemaleMinMatchedPlayerId { get; set; }
            public string FemaleMinPlayerName { get; set; }
        }

        public class GamePlayerMatchMinMaxReturn: GamePlayerMatchMinMaxData
        {

        }

        public class PlayerMatchSummaryData : GamePlayerDemos
        {
            public long MatchedPlayerId { get; set; }
            public string MatchedPlayerName { get; set; }
            public int NbrOfMatched { get; set; }
            public int NbrQuestions { get; set; }
        }

        public class PlayerMatchSummaryReturn
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public int Value { get; set; }
        }

        public class PlayerMatchDetailData : GamePlayerDemos
        {
            public long MatchedPlayerId { get; set; }
            public string MatchedPlayerName { get; set; }
            public long QuestionId { get; set; }
            public string Question { get; set; }
            public long OrderNbr { get; set; }
            public string PlayerChoice { get; set; }
            public string MatchedPlayerChoice { get; set; }
            public int Match { get; set; }
            public int PercentChosen { get; set; }
        }

        public class PlayerMatchDetailReturn
        {
            public string PlayerId { get; set; }
            public string PlayerName { get; set; }
            public long MatchedPlayerId { get; set; }
            public string MatchedPlayerName { get; set; }
            public long QuestionId { get; set; }
            public string Question { get; set; }
            public string PlayerChoice { get; set; }
            public string MatchedPlayerChoice { get; set; }
            public int Match { get; set; }
            public int PercentChosen { get; set; }
        }

        #endregion

        #region PlayerTest JSON Data Structures

        public class PlayerTestSummaryData : GamePlayerDemos
        {
            public int NbrQuestionsCorrect { get; set; }
            public int NbrQuestions { get; set; }
            public int PercentCorrect { get; set; }
        }

        public class PlayerTestSummaryReturn : PlayerMatchSummaryReturn
        {
        }

        public class PlayerTestDetailData : GamePlayerDemos
        {
            public long QuestionId { get; set; }
            public string Question { get; set; }
            public long OrderNbr { get; set; }
            public string SelectedChoices { get; set; }
            public string CorrectChoices { get; set; }
            public int QuestionCorrect { get; set; }
            public int PercentCorrect { get; set; }
        }

        public class PlayerTestDetailReturn
        {
            public string PlayerId { get; set; }
            public string PlayerName { get; set; }
            public long QuestionId { get; set; }
            public string Question { get; set; }
            public long OrderNbr { get; set; }
            public string SelectedChoices { get; set; }
            public string CorrectChoices { get; set; }
            public int QuestionCorrect { get; set; }
            public int PercentCorrect { get; set; }
        }

        #endregion


        #endregion

        #region PlayerMatch Reporting

        public JsonResult GetGamePlayerMatchMinMaxData(long gameplayid)
        {
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

        public ActionResult GamePlayerMatchMinMax(long gameplayid, int? mobileind)
        {
            ViewBag.GamePlayId = gameplayid;
            ViewBag.GamePlayName = db.GamePlay.Find(gameplayid).Name;
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"]; //get current selected game from GamePlay controller
            ViewBag.NbrQuestions = db.GamePlay.Find(gameplayid).Game.GameQuestions.Count();

            if (mobileind != 1)
            {
                ViewBag.MobileInd = false;
                return View();
            }
            else
            {
                ViewBag.MobileInd = true;
                return View("GamePlayerMatchMinMax", "_MobileLayout");
            }

        }

        public JsonResult GetPlayerMatchSummaryData(long gameplayid, long playerid)
        {
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

        public ActionResult PlayerMatchSummary(long gameplayid, long playerid, int? mobileind)
        {
            ViewBag.GamePlayId = gameplayid;
            ViewBag.GamePlayName = db.GamePlay.Find(gameplayid).Name;
            ViewBag.PlayerName = db.Player.Find(playerid).FirstName + " - " + db.Player.Find(playerid).NickName;
            ViewBag.PlayerId = playerid;

            ViewBag.NbrQuestions = db.GamePlay.Find(gameplayid).Game.GameQuestions.Count();

            if (mobileind != 1)
            {
                ViewBag.MobileInd = false;
                return View();
            }
            else
            {
                ViewBag.MobileInd = true;
                return View("PlayerMatchSummary", "_MobileLayout");
            }
        }

        public JsonResult GetPlayerMatchDetailData(long gameplayid, long playerid, long matchedplayerid)
        {
            int filterType = 0; //get all questions match or no-match

            var result = db.Database.SqlQuery<PlayerMatchDetailData>
                                             ("exec GetPlayerMatchDetail " + gameplayid + "," + playerid + ","
                                             + matchedplayerid + "," + filterType + ",'order by QTally.Match desc, OrderNbr asc'").ToList();


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

        public ActionResult PlayerMatchDetail(long gameplayid, long playerid, long matchedplayerid, int? mobileind)
        {
            ViewBag.GamePlayId = gameplayid;
            ViewBag.GamePlayName = db.GamePlay.Find(gameplayid).Name;
            ViewBag.PlayerId = playerid;
            ViewBag.MatchedPlayerId = matchedplayerid;

            if (mobileind != 1)
            {
                ViewBag.MobileInd = false;
                return View();
            }
            else
            {
                ViewBag.MobileInd = true;
                return View("PlayerMatchDetail", "_MobileLayout");
            }

        }

        #endregion

        #region PlayerTest Reporting

        public JsonResult GetPlayerTestSummaryData(long gameplayid)
        {
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

        public ActionResult PlayerTestSummary(long gameplayid, int? mobileind)
        {
            ViewBag.GamePlayId = gameplayid;
            ViewBag.GamePlayName = db.GamePlay.Find(gameplayid).Name;
            ViewBag.CurrentSelectedGame = Session["CurrentSelectedGame"];
            ViewBag.NbrQuestions = db.GamePlay.Find(gameplayid).Game.GameQuestions.Count();

            if (mobileind != 1)
            {
                ViewBag.MobileInd = false;
                return View();
            }
            else
            {
                ViewBag.MobileInd = true;
                return View("PlayerTestSummary", "_MobileLayout");
            }

        }

        public JsonResult GetPlayerTestDetailData(long gameplayid, long playerid)
        {

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

        public ActionResult PlayerTestDetail(long gameplayid, long playerid, int? mobileind)
        {
            ViewBag.GamePlayId = gameplayid;
            ViewBag.GamePlayName = db.GamePlay.Find(gameplayid).Name;
            ViewBag.PlayerId = playerid;

            //check mobile indicator from request
            if (mobileind != 1)
            {
                ViewBag.MobileInd = false;
                return View();
            }
            else
            {
                ViewBag.MobileInd = true;
                return View("PlayerTestDetail", "_MobileLayout");
            }

        }

        #endregion
    }

}