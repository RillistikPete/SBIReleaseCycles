using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StdBdgRCCL.Models
{
    public partial class Location
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("schoolId")]
        public long SchoolId { get; set; }

        [JsonProperty("classroomIdentificationCode")]
        public string ClassroomIdentificationCode { get; set; }

        [JsonProperty("_etag")]
        public string Etag { get; set; }
    }

    public partial class SchoolReference
    {
    }

    public partial class Link
    {
    }
}
