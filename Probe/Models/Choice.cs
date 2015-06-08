using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Probe.Helpers.Mics;

namespace Probe.Models
{
    public class Choice : IEntity
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [ForeignKey("ChoiceQuestion")]
        public long ChoiceQuestionId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: ProbeConstants.ChoiceNameMaxChars, MinimumLength = 2)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(maximumLength: ProbeConstants.ChoiceDescriptionMaxChars, MinimumLength = 2)]
        public string Text { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Order")]
        public long OrderNbr { get; set; }

        public bool Correct { get; set; }

        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedBy { get; set; }


        public virtual ChoiceQuestion ChoiceQuestion { get; set; }
    }
}