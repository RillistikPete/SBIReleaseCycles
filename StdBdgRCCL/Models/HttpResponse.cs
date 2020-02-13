using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace StdBdgRCCL.Models
{
    public class HttpResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public IEnumerable<T> ResponseContent { get; set; }
        public T RespContent { get; set; }

        public HttpResponse(bool statusCode, string message, IEnumerable<T> responseContent, T rcontent)
        {
            IsSuccess = statusCode;
            Message = message;
            ResponseContent = responseContent;
            RespContent = rcontent;
        }
    }
}
