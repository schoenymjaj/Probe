using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{
    public class ConfigurationG : IEntity
    {
        public enum ProbeDataType
        {
            TEXT,
            INT,
            FLOAT,
            BOOLEAN
        }

        public enum ProbeConfigurationType
        {
            GLOBAL,
            GAME
        }

        [Required]
        public long Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 60, MinimumLength = 5)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [StringLength(maximumLength: 20000, MinimumLength = 5)]
        public string Description { get; set; }

        [Required]
        public ProbeDataType DataTypeG { get; set; }

        [Required]
        public ProbeConfigurationType ConfigurationType { get; set; }

        [Required]
        [DataType(DataType.Html)]
        [StringLength(maximumLength: 200, MinimumLength = 1)]
        public string Value { get; set; }

        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<GameConfiguration> GameConfigurations { get; set; }
    }
}