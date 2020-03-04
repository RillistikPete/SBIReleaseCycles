using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models
{
    public class LogQueue
    {
        public int LogQueueId { get; set; }
        public DateTime DateAdded { get; set; }
        public string Message { get; set; }
        public string ProjectName { get; set; }
        public string Logger { get; set; }
    }
}
