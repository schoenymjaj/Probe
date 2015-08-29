using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models.View
{
    public class GameDTO 
    {

        public long Id { get; set; }

        public string AspNetUsersId { get; set; }

        public long GameTypeId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 60, MinimumLength = 5)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(maximumLength: 200, MinimumLength = 5)]
        public string Description { get; set; }

        [Required]
        public long ACLId { get; set; }

        //[Required]
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

        [Display(Name = "Published")]
        public bool Published { get; set; }

        public int PlayerCount { get; set; }

        public int PlayerActiveCount { get; set; }

        public int QuestionCount { get; set; }

        public bool IsActive { get; set; }

        public GameDTO()
        {
        }
    }
}