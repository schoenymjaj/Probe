using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models.View
{
    public class QuestionDTO
    {
        public long Id { get; set; }

        [Required]
        public long QuestionTypeId { get; set; }

        [Required]
        //[DataType(DataType.Text)] This data type sets the editable popup textbox for this field to be as wide as the popup (NOT GOOD)
        [StringLength(maximumLength: 60, MinimumLength = 5)]
        public string Name { get; set; }

        public string Text { get; set; }

        public bool TestEnabled { get; set; }

        public int ChoicesCount { get; set; }

        public bool Visible { get; set; }

        public virtual IEnumerable<ChoiceDTO> Choices { get; set; }

    }
}