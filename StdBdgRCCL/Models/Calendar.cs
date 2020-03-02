using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models
{
    public partial class Calendar
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("schoolReference")]
        public SchoolReference SchoolReference { get; set; }

        [JsonProperty("schoolYearTypeReference")]
        public SchoolYearTypeReference SchoolYearTypeReference { get; set; }

        [JsonProperty("calendarCode")]
        public long CalendarCode { get; set; }

        [JsonProperty("calendarTypeDescriptor")]
        public string CalendarTypeDescriptor { get; set; }

        [JsonProperty("gradeLevels")]
        public List<GradeLevel> GradeLevels { get; set; }

        [JsonProperty("_etag")]
        public string Etag { get; set; }
    }

    public partial class GradeLevel
    {
    }

    public partial class SchoolReference
    {
    }

    public partial class Link
    {
    }

    public partial class SchoolYearTypeReference
    {
    }
}
