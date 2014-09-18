using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Probe.Models
{
    public class Player : Person
    {

        [Required]
        [ForeignKey("GamePlay")]
        [Display(Name = "GamePlay")]
        public long GamePlayId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Submit Date")]
        public DateTime SubmitDate { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Submit Time")]
        public DateTime SubmitTime { get; set; }

        public virtual ICollection<GamePlayAnswer> GamePlayAnswers { get; set; }

        public virtual GamePlay GamePlay { get; set; }


    }
}