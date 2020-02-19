using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StdBdgRCCL.Models
{
    public class Log
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }
        public DateTime RowAdded { get; set; }
        public string AppName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public int SeverityLevel { get; set; }
        public string Category { get; set; }
        public string LoggedBy { get; set; }
    }
}
