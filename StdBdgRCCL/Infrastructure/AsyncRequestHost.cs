using Newtonsoft.Json;
using Polly;
using StdBdgRCCL.Models;
using StdBdgRCCL.Models.AzureDb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure
{
    public class AsyncRequestHost : DelegatingHandler
    {
        private const string _className = "AsyncRequestHost";
        private static CancellationToken cancellationToken;

        public static async Task<HttpResponse<List<T>>> SendRequestForListAsync<T>(HttpRequestMessage request, HttpClient client, string exceptionClientName)
        {
            const string _functionName = "SendRequestForListAsync<T>()";
            try
            {
                //var response = await SendAsync(request, cancellationToken);
                var response = await ExecuteSendRequestAsync(request, client);
                if (response.IsSuccessStatusCode)
                {
                    List<T> jsonResponse = new List<T>();
                    JsonConvert.PopulateObject(response.Content.ReadAsStringAsync().Result, jsonResponse);
                    //with constructor and inheritance
                    //var repoResponse = new HttpResponse<List<T>>(true, response.Content.ReadAsStringAsync().Result, jsonResponse );
                    //without:
                    var repoResponse = new HttpResponse<List<T>> { IsSuccess = true, ResponseContent = jsonResponse };
                    return repoResponse;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"{_functionName}: Request failed - {client.BaseAddress}/{request.RequestUri}  \r\n { _className}: {response.RequestMessage}");
                    LoggerLQ.LogQueue($"{_functionName}: Request failed - {client.BaseAddress}/{request.RequestUri} \r\n { _className}: {response.RequestMessage}");
                    List<T> jsonResponse = new List<T>();
                    //var repoResponse = new HttpResponse<List<T>>(true, response.Content.ReadAsStringAsync().Result, jsonResponse);
                    var repoResponse = new HttpResponse<List<T>> { IsSuccess = true, ResponseContent = jsonResponse };
                    return repoResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in {_className} - {_functionName}. {exceptionClientName}. Exception: {ex.Message}");
                LoggerLQ.LogQueue($"Exception in {_className} - {_functionName}. {exceptionClientName}. Exception: {ex.Message}");
                List<T> jsonResponse = new List<T>();
                //var repoResponse = new HttpResponse<List<T>>(true, null, jsonResponse);
                var repoResponse = new HttpResponse<List<T>> { IsSuccess = true, ResponseContent = jsonResponse };
                return repoResponse;
            }
        }

        public static async Task<HttpResponse<T>> SendRequestAsync<T>(HttpRequestMessage request, HttpClient client, string exceptionClientName) where T : new()
        {
            const string _functionName = "SendRequestAsync<T>()";
            try
            {
                var response = await ExecuteSendRequestAsync(request, client);
                if (response.IsSuccessStatusCode)
                {
                    T jsonResponse = new T();
                    //List<T> jrList = new List<T>();
                    var settings = new JsonSerializerSettings
                    {
                        DateParseHandling = DateParseHandling.DateTimeOffset,
                        NullValueHandling = NullValueHandling.Ignore,
                    };
                    var jRslt = response.Content.ReadAsStringAsync().Result;
                    //if (jsonResponse.GetType() == typeof(BadgeIntegration.Models.EdFiV3Student))
                    if (jRslt[0] == '[')
                    {
                        var dobj = JsonConvert.DeserializeObject<T[]>(jRslt);
                        var y = dobj.First();
                        var szObj = JsonConvert.SerializeObject(y);
                        JsonConvert.PopulateObject(szObj, jsonResponse, settings);
                    }
                    else
                    {
                        JsonConvert.PopulateObject(jRslt, jsonResponse);
                    }
                    return new HttpResponse<T> { IsSuccess = true, ResponseContent = jsonResponse };
                    //JsonConvert.PopulateObject(jRslt, jsonResponse);
                    //JsonConvert.PopulateObject(jRslt, jrList);
                    //return new HttpResponse<T> { IsSuccess = true, ResponseContent = jrList[0] != null ? jrList[0] : jrList[1] };
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"{_functionName}: Request failed - {client.BaseAddress}/{request.RequestUri}  \r\n { _className}: {response.RequestMessage}");
                    LoggerLQ.LogQueue($"{_functionName}: Request failed - {client.BaseAddress}/{request.RequestUri}  \r\n { _className}: {response.RequestMessage}");
                    return new HttpResponse<T> { IsSuccess = true, StatusCode = HttpStatusCode.BadRequest };
                }
            }
            catch (Exception exc)
            {
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}. Exception: {exc.Message}");
                Console.WriteLine($"Exception in {_className} at {_functionName}. Exception: {exc.Message}");
                return new HttpResponse<T> { IsSuccess = true, StatusCode = HttpStatusCode.BadRequest };
            }
        }

        public static async Task<ServerResponse> SendPropagateRequestAsync(HttpRequestMessage request, HttpClient client, string exceptionClientName)
        {
            const string _functionName = "SendPropagateRequestAsync()";
            try
            {
                var response = await ExecuteSendRequestAsync(request, client);
                return new ServerResponse
                {
                    HttpRespMsg = response,
                    Message = await response.Content.ReadAsStringAsync()
                };
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Exception in {_className} at {_functionName}. Exception: {exc.Message}");
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}. Exception: {exc.Message}");
                return new ServerResponse
                {
                    HttpRespMsg = new HttpResponseMessage(HttpStatusCode.BadRequest),
                    Message = $"Exception in {_className} at {_functionName}. Exception: {exc.Message}"
                };
            }
        }

        public static async Task<HttpResponseMessage> ExecuteSendRequestAsync(HttpRequestMessage request, HttpClient client)
        {
            const string _functionName = "ExecuteSendRequestAsync()";
            try
            {
                var response = await client.SendAsync(request);
                return response;
            }
            catch (WebException exc)
            {
                Console.WriteLine($"Exception in {_functionName}: {exc.Message}");
                throw exc;
            }
        }

        public static HttpRequestMessage GetNewRequestMessage(HttpRequestMessage request)
        {
            HttpRequestMessage newRequest = new HttpRequestMessage(request.Method, request.RequestUri);

            if (request.Content != null)
            {
                newRequest.Content = request.Content;
            }
            if (request.Properties != null)
            {
                foreach (var prop in request.Properties)
                    newRequest.Properties.Add(prop.Key, prop.Value);
            }
            return newRequest;
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage reqMsg, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            //var policy = Policy
            var response = await base.SendAsync(reqMsg, cancellationToken);
            Console.WriteLine($"Completed request in {sw.ElapsedMilliseconds}ms.");
            return response;
        }
    }
}
