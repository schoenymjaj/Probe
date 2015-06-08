using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models
{
    public abstract class Person : IEntity
    {
        public enum SexType
        {
            UNKNOWN,
            MALE,
            FEMALE
        }

        //[Required]
        public long Id { get; set; }

        //[Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName + " " + MiddleName;
            }
        }

        [StringLength(1)]
        [Display(Name = "Middle Initial")]
        public string MiddleName { get; set; }

        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Column("NickName")]
        [Display(Name = "Nick Name")]
        public string NickName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddr { get; set; }

        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:###-###-####}",ApplyFormatInEditMode = true)]
        public string MobileNbr { get; set; }

        public SexType Sex { get; set; }

        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string UpdatedBy { get; set; }

    }
}