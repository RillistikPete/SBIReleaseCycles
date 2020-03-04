using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models
{
    public class StudentSchedule
    {
        [JsonProperty("personID")]
        public int? PersonID { get; set; }

        [JsonProperty("studentNumber")]
        public string StudentNumber { get; set; }

        [JsonProperty("course")]
        public string Course { get; set; }

        [JsonProperty("schoolYear")]
        public string SchoolYear { get; set; }

        [JsonProperty("term")]
        public string Term { get; set; }

        [JsonProperty("edfiTerm")]
        public string EdfiTerm { get; set; }

        [JsonProperty("termStart")]
        public DateTime? TermStart { get; set; }

        [JsonProperty("termEnd")]
        public DateTime? TermEnd { get; set; }

        [JsonProperty("period")]
        public string Period { get; set; }

        [JsonProperty("periodSequence")]
        public int? PeriodSequence { get; set; }

        [JsonProperty("scheduleSequence")]
        public int? ScheduleSequence { get; set; }

        [JsonProperty("teacher")]
        public string Teacher { get; set; }

        [JsonProperty("homeroom")]
        public bool Homeroom { get; set; }
        //addition 12/16/19
        [JsonProperty("homeroomSection")]
        public bool? HomeroomSection { get; set; }

        [JsonProperty("calendarName")]
        public string CalendarName { get; set; }

        [JsonProperty("calendarID")]
        public int? CalendarID { get; set; }
    }
}
