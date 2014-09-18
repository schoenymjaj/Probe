using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{
    public class GamePlayReport : IEntity
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [ForeignKey("GamePlay")]
        public long GamePlayId { get; set; }

        [Required]
        [ForeignKey("ReportType")]
        public long ReportTypeId { get; set; }

        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedBy { get; set; }

        public virtual GamePlay GamePlay { get; set; }

        public virtual ReportType ReportType { get; set; }

    }
}