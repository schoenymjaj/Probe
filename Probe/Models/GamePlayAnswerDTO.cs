using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{
    public class GamePlayAnswerDTO
    {

        public long Id { get; set; }

        public long PlayerId { get; set; }

        public long ChoiceId { get; set; }

    }
}