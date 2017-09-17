using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProbeDAL.Models;


namespace Probe.Models.View
{
    public class PlayerDTO
    {
        public long Id { get; set; }

        [Required]
        public long GameId { get; set; }

        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(1)]
        [Display(Name = "Middle Initial")]
        public string MiddleName { get; set; }

        [StringLength(50)]
        [Display(Name = "Nick Name")]
        public string NickName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string EmailAddr { get; set; }

        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:###-###-####}", ApplyFormatInEditMode = true)]
        public string MobileNbr { get; set; }

        public Person.SexType Sex { get; set; }

        [Display(Name = "Submit Date")]
        public DateTime SubmitDate { get; set; }

        [Display(Name = "Submit Time")]
        public DateTime SubmitTime { get; set; }

        [Display(Name = "Submit Date")]
        public DateTime SubmitDateTime { get; set; }

        public bool Active { get; set; }

        public int PlayerGameReason { get; set; }

        public string PlayerGameName { get; set; }

    }
}