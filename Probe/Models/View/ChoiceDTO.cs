using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProbeDAL;

namespace Probe.Models.View
{
    public class ChoiceDTO
    {
        public long Id { get; set; }

        public long ChoiceQuestionId { get; set; }

        public long ACLId { get; set; }

        public string Name { get; set; }

        [StringLength(maximumLength: ProbeDALTypes.ChoiceDescriptionMaxChars, MinimumLength = 2)]
        public string Text { get; set; }

        public bool Correct { get; set; }

        [Range(1, 100)]
        public long OrderNbr { get; set; }

    }
}