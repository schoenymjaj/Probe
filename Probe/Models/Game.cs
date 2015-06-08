using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{
    public class Game : IEntity
    {

        [Required]
        public long Id { get; set; }

        [Required(ErrorMessage="A user needs to be logged in to create a Game.")]
        [StringLength(maximumLength: 128)]
        public string AspNetUsersId { get; set; }

        [Required]
        [ForeignKey("GameType")]
        [Display(Name = "Type")]
        public long GameTypeId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: 60, MinimumLength = 5)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(maximumLength: 200, MinimumLength = 5)]
        public string Description { get; set; }

        [Required]
        [ForeignKey("ACL")]
        [Display(Name = "ACL")]
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

        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedBy { get; set; }

        public virtual GameType GameType { get; set; }

        public virtual ACL ACL { get; set; }

        public virtual ICollection<GameQuestion> GameQuestions { get; set; }

        public virtual ICollection<GameConfiguration> GameConfigurations { get; set; }

        public virtual ICollection<Player> Players { get; set; }

        public Game()
        {
        }
    }
}