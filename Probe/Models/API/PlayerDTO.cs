﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProbeDAL.Models;


namespace Probe.Models.API
{
    public class PlayerDTO
    {
        public long Id { get; set; }

        [Required]
        public long GameId { get; set; }

        [Required]
        public string GameCode { get; set; }

        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(1)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(50)]
        public string NickName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddr { get; set; }

        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName + " " + MiddleName;
            }
        }

        public string PlayerGameName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:###-###-####}", ApplyFormatInEditMode = true)]
        public string MobileNbr { get; set; }

        public Person.SexType Sex { get; set; }

        public string ClientVersion { get; set; }

        public virtual ICollection<GameAnswerDTO> GameAnswers { get; set; } //MNS - 2/8/15 support one POST approach

        public virtual PlayerGameStatus PlayerGameStatus { get; set; }

    }
}