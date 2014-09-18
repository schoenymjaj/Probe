using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{
    public class ChoiceQuestion : Question
    {
        [Display(Name = "One Choice")]
        public bool OneChoice { get; set; }

        public virtual ICollection<Choice> Choices { get; set; }

    }
}