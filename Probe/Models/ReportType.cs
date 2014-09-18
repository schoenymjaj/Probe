using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{
    public class ReportType : IEntity
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 60, MinimumLength = 5)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(maximumLength: 200, MinimumLength = 5)]
        public string Description { get; set; }

        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<GamePlay> GamePlays { get; set; }

    }
}