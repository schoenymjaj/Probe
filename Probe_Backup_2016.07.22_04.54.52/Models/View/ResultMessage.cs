using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProbeDAL.Models;
using Probe.Models.View;
using Probe.Helpers.Mics;


namespace Probe.Models.View
{
    public class ResultMessage
    {
        public int MessageId { get; set; }
        public MessageType MessageType { get; set; }
        public string Message { get; set; }
    }
}