using Newtonsoft.Json;
using StdBdgRCCL.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure
{
    class AsyncRequestHost
    {
        public static async Task<HttpResponse<List<T>>> SendRequestForListAsync<T>(HttpRequestMessage request, HttpClient client, string exceptionClientName)
        {
            try
            {
                var response = await ExecuteSendRequestAsync(request, client);


                if (response.IsSuccessStatusCode)
                {
                    List<T> jsonResponse = new List<T>();
                    JsonConvert.PopulateObject(response.Content.ReadAsStringAsync().Result, jsonResponse);
                    var repoResponse = new HttpResponse<List<T>> { IsSuccessStatusCode = true, Content = jsonResponse };
                    return repoResponse;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Logger.Log($"Request failed - [[{request}]]",
                               $"{response.Headers} \r\n" +
                               $"{responseContent}");

                    List<T> jsonResponse = new List<T>();
                    var repoResponse = new HttpResponse<List<T>> { IsSuccessStatusCode = false, Content = jsonResponse };
                    return repoResponse;

                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception in {exceptionClientName}", $"{ex}");
                List<T> jsonResponse = new List<T>();
                var repoResponse = new HttpResponse<List<T>> { IsSuccessStatusCode = false, Content = jsonResponse };
                return repoResponse;
            }
        }

        public static async Task<HttpResponse<T>> SendRequestAsync<T>(HttpRequestMessage request, HttpClient client, string exceptionClientName) where T : new()
        {
            try
            {
                var response = await ExecuteSendRequestAsync(request, client);
                if (response.IsSuccessStatusCode)
                {
                    T jsonResponse = new T();
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
                    var repoResponse = new RepoResponse<T> { IsSuccessStatusCode = true, Content = jsonResponse };

                    return repoResponse;

                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Logger.Log($"Request failed - [[{request}]]",
                               $"{response.Headers} \r\n" +
                               $"{responseContent}");

                    T jsonResponse = new T();
                    var repoResponse = new RepoResponse<T> { IsSuccessStatusCode = false, Content = jsonResponse };

                    return repoResponse;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception in {exceptionClientName}", $"{ex}");
                Console.WriteLine($"Exception in {exceptionClientName}", $"{ex}");


                T jsonResponse = new T();
                var repoResponse = new RepoResponse<T> { IsSuccessStatusCode = false, Content = jsonResponse };

                return repoResponse;
            }
        }

        public static async Task<ServerResponse> SendPropagateRequestAsync(HttpRequestMessage request, HttpClient client, string exceptionClientName)
        {
            ServerResponse serverResponse = new ServerResponse();

            try
            {
                var response = await ExecuteSendRequestAsync(request, client);

                serverResponse.Response = response;
                serverResponse.Message = await response.Content.ReadAsStringAsync();
                return serverResponse;
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception in {exceptionClientName}", $"{ex}");

                HttpResponseMessage response = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest };
                serverResponse.Response = response;
                return serverResponse;
            }
        }

        public static async Task<HttpResponseMessage> ExecuteSendRequestAsync(HttpRequestMessage request, HttpClient client)
        {
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
                    Console.WriteLine($"Request unsuccessful. [[{request}]] - Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
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
    }
}
