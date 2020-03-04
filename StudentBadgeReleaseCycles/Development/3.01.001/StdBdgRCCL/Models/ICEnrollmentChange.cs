using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models
{
    public class ICEnrollmentChange
    {
        [JsonProperty("enrollmentID")]
        public long EnrollmentId { get; set; }

        [JsonProperty("personID")]
        public long PersonId { get; set; }

        [JsonProperty("calendarID")]
        public long CalendarId { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }

        [JsonProperty("serviceType")]
        public string ServiceType { get; set; }

        [JsonProperty("entryDate")]
        public DateTime? EntryDate { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonProperty("exitDate")]
        public DateTime? ExitDate { get; set; }

        [JsonProperty("structureID")]
        public long StructureId { get; set; }
    }
}
