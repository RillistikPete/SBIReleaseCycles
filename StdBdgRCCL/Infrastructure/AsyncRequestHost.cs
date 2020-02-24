using Newtonsoft.Json;
using Polly;
using StdBdgRCCL.Models;
using StdBdgRCCL.Models.AzureDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure
{
    public class AsyncRequestHost
    {
        private const string _className = "AsyncRequestHost";

        public static async Task<HttpResponse<List<T>>> SendRequestForListAsync<T>(HttpRequestMessage request, HttpClient client, string exceptionClientName)
        {
            const string _functionName = "SendRequestForListAsync<T>()";
            try
            {
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
                    Console.WriteLine($"{_functionName}: Request failed - {request}  \r\n { _className} at { _functionName}");
                    LoggerLQ.LogQueue($"{_functionName}: Request failed - {request}  \r\n { _className} at { _functionName}");
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
                    Console.WriteLine($"{_functionName}: Request failed - {request}  \r\n { _className} at { _functionName}");
                    LoggerLQ.LogQueue($"{_functionName}: Request failed - {request}  \r\n { _className} at { _functionName}");
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
                var httpRetryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(result => !result.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(2), async (responseMessage, timeSpan, retryCount, context) =>
                {
                    var requestContent = responseMessage.Result.RequestMessage.Content != null ? await responseMessage.Result.RequestMessage.Content.ReadAsStringAsync() : "";
                    Console.WriteLine(requestContent);
                    var responseContent = await responseMessage.Result.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                    if (retryCount == 3) { }
                    Console.WriteLine($"{_functionName} - Request unsuccessful: {client.BaseAddress} \r\n" +
                                        $" [[{request}]] \r\n" +
                                        $"Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                });

                var httpFallbackPolicy = Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .OrResult(result => !result.IsSuccessStatusCode)
                    .FallbackAsync(async fallback =>
                    {
                        if (request.Properties.ContainsKey("studentUniqueId"))
                        {
                            return new HttpResponseMessage()
                            {
                                StatusCode = HttpStatusCode.BadRequest,
                                Content = new StringContent("Fallback policy used, studentUniqueId queued for retry")
                            };
                        }
                        else
                        {
                            return new HttpResponseMessage()
                            {
                                StatusCode = HttpStatusCode.BadRequest,
                                Content = new StringContent("Fallback policy used.")
                            };
                        }
                    });

                var response = await Policy.WrapAsync(httpFallbackPolicy, httpRetryPolicy).ExecuteAsync(() => client.SendAsync(GetNewRequestMessage(request)));
                return response;
            }
            catch (WebException ex)
            {
                throw ex;
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
    }
}
