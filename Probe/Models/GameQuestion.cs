using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{
    public class GameQuestion : IEntity
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [ForeignKey("Game")] //MNS NEXT MIGRATION STEP
        public long GameId { get; set; }

        [Required]
        [ForeignKey("Question")]
        public long QuestionId { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Order")]
        public long OrderNbr { get; set; }

        [Range(1, 10)]
        public int Weight { get; set; }

        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Game Game { get; set; }
        public virtual Question Question { get; set; }


    }
}