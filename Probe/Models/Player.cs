using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Probe.Models
{
    public class Player : Person
    {

        public enum PlayerGameReasonType
        {
            UNKNOWN = 0,
            ANSWER_REASON_INCORRECT = 1,
            ANSWER_REASON_DEADLINE = 2,
            ANSWER_REASON_UNKNOWN = 3
        }

        [Required]
        [ForeignKey("Game")]
        [Display(Name = "Game")]
        public long GameId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Submit Date")]
        public DateTime SubmitDate { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Submit Time")]
        public DateTime SubmitTime { get; set; }

        public bool Active { get; set; }

        public PlayerGameReasonType PlayerGameReason { get; set; }
        
        public virtual ICollection<GameAnswer> GameAnswers { get; set; }

        public virtual Game Game { get; set; }


    }
}