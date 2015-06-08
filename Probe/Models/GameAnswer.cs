using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Probe.Models
{
    public class GameAnswer : IEntity
    {
        //[Required]
        public long Id { get; set; }

        [Required]
        [ForeignKey("Player")]
        public long PlayerId { get; set; }

        [Required]
        [ForeignKey("Choice")]
        public long ChoiceId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Player Player { get; set; }

        public virtual Choice Choice { get; set; }

    }
}