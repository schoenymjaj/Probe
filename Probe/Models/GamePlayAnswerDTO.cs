using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{
    public class GamePlayAnswerDTO
    {

        public long Id { get; set; }

        [Required]
        public string GameCode { get; set; }

        [Required]
        public long PlayerId { get; set; }

        [Required]
        public long ChoiceId { get; set; }

    }
}