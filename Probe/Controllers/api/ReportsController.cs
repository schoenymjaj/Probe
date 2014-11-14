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
using Probe.Helpers.Exceptions;

namespace Probe.Controllers.api
{
    public class ReportsController : ApiController
    {
        private ProbeDataContext db = new ProbeDataContext();

        [Route("api/Reports/GetGamePlayerMatchMinMaxData/{gameplayid}/{code}")]
        public List<GamePlayerMatchMinMaxReturn> GetGamePlayerMatchMinMaxData(long gameplayid, string code)
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

            return reportData;
        }

        [Route("api/Reports/GetPlayerMatchSummaryData/{gameplayid}/{code}/{playerid}")]
        public List<PlayerMatchSummaryReturn> GetPlayerMatchSummaryData(long gameplayid, string code, long playerid)
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

            return reportData;
        }

        [Route("api/Reports/GetPlayerMatchDetailData/{gameplayid}/{code}/{playerid}/{matchedplayerid}")]
        public List<PlayerMatchDetailReturn> GetPlayerMatchDetailData(long gameplayid, string code, long playerid, long matchedplayerid)
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

            return reportData;
        }

        [Route("api/Reports/GetPlayerTestSummaryData/{gameplayid}/{code}")]
        public List<PlayerTestSummaryReturn> GetPlayerTestSummaryData(long gameplayid, string code)
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

            return reportData;
        }

        [Route("api/Reports/GetPlayerTestDetailData/{gameplayid}/{code}/{playerid}")]
        public List<PlayerTestDetailReturn> GetPlayerTestDetailData(long gameplayid, string code, long playerid)
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

            return reportData;
        }


    }
}