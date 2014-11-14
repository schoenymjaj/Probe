using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{

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

    public class GamePlayerMatchMinMaxReturn : GamePlayerMatchMinMaxData
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


}