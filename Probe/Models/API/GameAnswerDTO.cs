using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models.API
{
    public class GameAnswerDTO
    {

        public long Id { get; set; }

        [Required]
        public string GameCode { get; set; }

        public int QuestionNbr { get; set; }

        public long QuestionId { get; set; }

        [Required]
        public long PlayerId { get; set; }

        [Required]
        public long ChoiceId { get; set; } 

    }
}