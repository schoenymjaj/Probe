using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{
    public class GamePlay : IEntity
    {

        [Required]
        public long Id { get; set; }

        [Required]
        [ForeignKey("Game")]
        public long GameId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 60, MinimumLength = 5)]
        [Display(Name = "Game Play")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(maximumLength: 200, MinimumLength = 5)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 60, MinimumLength = 5)]
        public string Code { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Game URL")]
        public string GameUrl { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Suspend Mode")]
        public bool SuspendMode { get; set; }

        [Display(Name = "Client Report Access")]
        public bool ClientReportAccess { get; set; }

        [Display(Name = "Test Mode")]
        public bool TestMode { get; set; }

        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Game Game { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        public virtual ICollection<GamePlayReport> GamePlayReports { get; set; }

        public GamePlay()
        {
            StartDate = DateTime.Now;
            EndDate = StartDate.Add(System.TimeSpan.FromDays(1));
        }
    }
}