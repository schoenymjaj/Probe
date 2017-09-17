using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models.View
{
    public class GameQuestionDTO
    {
        public long Id { get; set; }

        public long GameId { get; set; }

        public long QuestionId { get; set; }

        public long QuestionTypeId { get; set; }

        public long OrderNbr { get; set; }

        public int Weight { get; set; }

        [StringLength(maximumLength: 60, MinimumLength = 5)]
        public string Name { get; set; }

        public string Text { get; set; }

        public bool TestEnabled { get; set; }

        public int ChoicesCount { get; set; }

    }
}