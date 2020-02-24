using Newtonsoft.Json;
using StdBdgRCCL.Interfaces;
using StdBdgRCCL.Models;
using StdBdgRCCL.Models.AzureDb;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure.ClientBase
{
    public class ICClientBase : IICClient
    {
        private readonly HttpClient _icClient;
        private const string _clientName = "ICClient";
        private const string _className = "ICClientBase";
        public ICClientBase(HttpClient client)
        {
            _icClient = client;
        }

        public async Task<HttpResponse<List<T>>> Get<T>(string resourceUri, int offset = 0, int pagesize = 1000, IDictionary<string, string> properties = null)
        {
            const string _functionName = "Get<T>()";
            try
            {
                var fullResourceUri = $"{resourceUri}?offset={offset}&pagesize={pagesize}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                return await AsyncRequestHost.SendRequestForListAsync<T>(request, _icClient, _clientName);
            }
            catch (Exception exc)
            {
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}, \r\n {exc.Message}");
                return new HttpResponse<List<T>> { IsSuccess = false };
            }
        }

        public async Task<HttpResponse<List<T>>> GetById<T>(string resourceUri, string id, IDictionary<string, string> properties = null)
        {
            const string _functionName = "GetById<T>()";
            try
            {
                var fullResourceUri = $"{resourceUri}{id}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                return await AsyncRequestHost.SendRequestForListAsync<T>(request, _icClient, _clientName);
            }
            catch (Exception exc)
            {
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}, \r\n {exc.Message}");
                return new HttpResponse<List<T>> { IsSuccess = false };
            }
        }

        public async Task<HttpResponse<List<T>>> GetByExample<T>(string resourceUri, IDictionary<string, string> properties = null, int offset = 0, int pagesize = 100)
        {
            const string _functionName = "GetByExample<T>()";
            try
            {
                var fullResourceUri = $"{resourceUri}&offset={offset}&pagesize={pagesize}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                if (properties != null)
                {
                    foreach (var prop in properties)
                        request.Properties.Add(prop.Key, prop.Value);
                }
                return await AsyncRequestHost.SendRequestForListAsync<T>(request, _icClient, _clientName);
            }
            catch (Exception ex)
            {
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}, \r\n {ex.Message}");
                return new HttpResponse<List<T>> { IsSuccess= false };
            }
        }

        public async Task<HttpResponse<T>> GetSingleByExample<T>(string resourceUri, IDictionary<string, string> properties = null) where T : new()
        {
            const string _functionName = "GetSingleByExample<T>()";
            try
            {
                var fullResourceUri = $"{resourceUri}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                if (properties != null)
                {
                    foreach (var prop in properties)
                        request.Properties.Add(prop.Key, prop.Value);
                }
                return await AsyncRequestHost.SendRequestAsync<T>(request, _icClient, _clientName);
            }
            catch (Exception exc)
            {
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}, \r\n {exc.Message}");
                return new HttpResponse<T> { IsSuccess = false };
            }
        }

        public async Task<ServerResponse> Put(string resourceUri, string id, dynamic dto, IDictionary<string, string> properties = null)
        {
            const string _functionName = "Put()";
            try
            {
                var json = JsonConvert.SerializeObject(dto, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{resourceUri}{id}")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
                return await AsyncRequestHost.SendPropagateRequestAsync(request, _icClient, _clientName);
            }
            catch (Exception exc)
            {
                return new ServerResponse
                {
                    HttpRespMsg = new HttpResponseMessage(HttpStatusCode.BadRequest),
                    Message = $"Exception in {_className} at {_functionName}; {exc.Message}"
                };
            }
        }

        public async Task<ServerResponse> Delete(string resourceUri, string id, string checksum, IDictionary<string, string> properties = null)
        {
            const string _functionName = "Delete()";
            try
            {
                var fullResourceUri = $"{resourceUri}{id}?checksum={checksum}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, fullResourceUri);
                return await AsyncRequestHost.SendPropagateRequestAsync(request, _icClient, _clientName);
            }
            catch (Exception exc)
            {
                return new ServerResponse
                {
                    HttpRespMsg = new HttpResponseMessage(HttpStatusCode.BadRequest),
                    Message = $"Exception in {_className} at {_functionName}; {exc.Message}"
                };
            }

        }

        public async Task<ServerResponse> Post(string resourceUri, dynamic dto, IDictionary<string, string> properties = null)
        {
            const string _functionName = "Post()";
            try
            {
                var fullResourceUri = $"{resourceUri}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, fullResourceUri)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(dto, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json")
                };
                return await AsyncRequestHost.SendPropagateRequestAsync(request, _icClient, _clientName);
            }
            catch (Exception exc)
            {
                return new ServerResponse
                {
                    HttpRespMsg = new HttpResponseMessage(HttpStatusCode.BadRequest),
                    Message = $"Exception in {_className} at {_functionName}; {exc.Message}"
                };
            }
        }
    }
}
