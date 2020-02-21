using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace StdBdgRCCL.Models
{
    public class HttpResponse<T> : HttpResponseMessage
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T ResponseContent { get; set; }

        //public HttpResponse(bool statusCode, string message, T responseContent)
        //{
        //    IsSuccess = statusCode;
        //    Message = message;
        //    ResponseContent = responseContent;
        //}
    }
}
