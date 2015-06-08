using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Probe.Models;


namespace Probe.Models
{
    public class PlayerGameStatus
    {
        [Required]
        public long GameId { get; set; }

        public bool PlayerActive { get; set; }

        public int NbrPlayers { get; set; }

        public int NbrPlayersRemaining { get; set; }

        public int NbrAnswersCorrect { get; set; }

        public int MessageId { get; set; }

        public string Message { get; set; }
    }
}