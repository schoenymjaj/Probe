using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Probe.Models.View
{
    public class GameQuestionScheduleDTO : GameQuestionDTO
    {

        public string ScheduleName { get; set; }

        public string ScheduleDesc { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Interim Date")]
        public DateTime InterimDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Time Span")]
        public string TimeSpanString { get; set; }

        public GameQuestionScheduleDTO()
        {
        }
    }
}