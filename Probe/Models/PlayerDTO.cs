using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Probe.Models;


namespace Probe.Models
{
    public class PlayerDTO
    {
        public long Id { get; set; }

        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName + " " + MiddleName;
            }
        }

        [StringLength(1)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(50)]
        public string NickName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddr { get; set; }

        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:###-###-####}", ApplyFormatInEditMode = true)]
        public string MobileNbr { get; set; }

        public Person.SexType Sex { get; set; }


    }
}