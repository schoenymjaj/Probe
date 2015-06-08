using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Probe.Helpers.Mics;

namespace Probe.Models
{
    public class Question : IEntity
    {

        [Required]
        public long Id { get; set; }

        [Required(ErrorMessage = "A user needs to be logged in to create a Question.")]
        [StringLength(maximumLength: 128)]
        public string AspNetUsersId { get; set; }

        [Required]
        [ForeignKey("QuestionType")]
        [Display(Name = "Type")]
        public long QuestionTypeId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(maximumLength: ProbeConstants.QuestionNameMaxChars, MinimumLength = 5)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(maximumLength: ProbeConstants.QuestionDescriptionMaxChars, MinimumLength = 5)]
        public string Text { get; set; }

        public bool UsedInGame { get; set; }

        [Required]
        [ForeignKey("ACL")]
        [Display(Name = "ACL")]
        public long ACLId { get; set; }


        public virtual QuestionType QuestionType { get; set; }

        public virtual ACL ACL { get; set; }

        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<GameQuestion> GameQuestions { get; set; }

    }
}