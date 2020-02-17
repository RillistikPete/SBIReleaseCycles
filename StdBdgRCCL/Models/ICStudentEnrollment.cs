using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models
{
    public class ICStudentEnrollment
    {
        [JsonProperty("personID")]
        public long PersonId { get; set; }

        [JsonProperty("stateStudentNumber")]
        public long? StateStudentNumber { get; set; }

        [JsonProperty("studentNumber")]
        public long StudentNumber { get; set; }

        [JsonProperty("edFiID")]
        public long EdFiId { get; set; }

        [JsonProperty("lastModified")]
        public DateTimeOffset LastModified { get; set; }

        [JsonProperty("entryDate")]
        public DateTimeOffset EntryDate { get; set; }

        [JsonProperty("exitDate")]
        public DateTimeOffset? ExitDate { get; set; }

        [JsonProperty("enrollmentType")]
        public string EnrollmentType { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }

        [JsonProperty("schoolID")]
        public long SchoolId { get; set; }

        [JsonProperty("calendarID")]
        public long CalendarId { get; set; }

        [JsonProperty("schoolName")]
        public string SchoolName { get; set; }

        [JsonProperty("localSchoolNumber")]
        public long LocalSchoolNumber { get; set; }

        [JsonProperty("stateSchoolNumber")]
        public string StateSchoolNumber { get; set; }

        [JsonProperty("calendarName")]
        public string CalendarName { get; set; }

        [JsonProperty("schoolStartYear")]
        public long SchoolStartYear { get; set; }

        [JsonProperty("schoolEndYear")]
        public long SchoolEndYear { get; set; }

        [JsonProperty("schoolYearName")]
        public string SchoolYearName { get; set; }
    }
}
