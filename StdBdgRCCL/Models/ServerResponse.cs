using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace StdBdgRCCL.Models
{
    public class ServerResponse
    {
        public HttpResponseMessage HttpRespMsg { get; set; }
        public string Message { get; set; }
    }
}
