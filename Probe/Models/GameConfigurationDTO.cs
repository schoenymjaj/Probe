using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{
    public class GameConfigurationDTO
    {
        public long? Id { get; set; }

        [Required]
        public long ConfigurationGId { get; set; }

        public long? GameId { get; set; }

        public string GameName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 60, MinimumLength = 5)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [StringLength(maximumLength: 20000, MinimumLength = 5)]
        public string Description { get; set; }

        [Required]
        public Probe.Models.ConfigurationG.ProbeDataType DataTypeG { get; set; }

        [Required]
        public Probe.Models.ConfigurationG.ProbeConfigurationType ConfigurationType { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 200, MinimumLength = 1)]
        public string Value { get; set; }

    }
}