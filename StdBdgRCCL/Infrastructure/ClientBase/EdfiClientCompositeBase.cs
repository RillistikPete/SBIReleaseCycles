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
    public class EdfiClientCompositeBase : IEdfiClient
    {
        private const string _clientName = "EdFiCompositeClient";
        private const string _className = "EdFiClientCompositeBase";
        public static HttpClient edfiClientComp;
        public EdfiClientCompositeBase(HttpClient client)
        {
            edfiClientComp = client;
        }

        public async Task<HttpResponse<List<T>>> Get<T>(string resourceUri, int offset = 0, int limit = 100, IDictionary<string, string> properties = null)
        {
            const string _functionName = "Get<T>()";
            try
            {
                var fullResourceUri = $"{resourceUri}?offset={offset}&limit={limit}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                return await AsyncRequestHost.SendRequestForListAsync<T>(request, edfiClientComp, _clientName);
            }
            catch (Exception exc)
            {
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}, \r\n {exc.Message}");
                return new HttpResponse<List<T>> { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
            }
        }

        public async Task<HttpResponse<List<T>>> GetByExample<T>(string resourceUri, IDictionary<string, string> properties = null, int offset = 0, int limit = 100)
        {
            const string _functionName = "GetByExample<T>()";
            var fullResourceUri = "";
            try
            {
                if (!resourceUri.Contains("?"))
                {
                    fullResourceUri = $"{resourceUri}?offset={offset}&limit={limit}";
                }
                else
                {
                    fullResourceUri = $"{resourceUri}&offset={offset}&limit={limit}";
                }
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                if (properties != null)
                {
                    foreach (var prop in properties)
                        request.Properties.Add(prop.Key, prop.Value);
                }
                return await AsyncRequestHost.SendRequestForListAsync<T>(request, edfiClientComp, _clientName);
            }
            catch (Exception exc)
            {
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}, \r\n {exc.Message}");
                return new HttpResponse<List<T>> { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
            }
        }

        public async Task<HttpResponse<List<T>>> GetByExampleLowerLimit<T>(string resourceUri, IDictionary<string, string> properties = null, int offset = 0, int limit = 25)
        {
            const string _functionName = "GetByExampleLowerLimit<T>()";
            var fullResourceUri = "";
            try
            {
                if (!resourceUri.Contains("?"))
                {
                    fullResourceUri = $"{resourceUri}?offset={offset}&limit={limit}";
                }
                else
                {
                    fullResourceUri = $"{resourceUri}&offset={offset}&limit={limit}";
                }
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                if (properties != null)
                {
                    foreach (var prop in properties)
                        request.Properties.Add(prop.Key, prop.Value);
                }
                return await AsyncRequestHost.SendRequestForListAsync<T>(request, edfiClientComp, _clientName);
            }
            catch (Exception exc)
            {
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}, \r\n {exc.Message}");
                return new HttpResponse<List<T>> { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
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
                return await AsyncRequestHost.SendRequestAsync<T>(request, edfiClientComp, _clientName);
            }
            catch (Exception exc)
            {
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}, \r\n {exc.Message}");
                return new HttpResponse<T> { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
            }
        }

        public async Task<HttpResponse<T>> GetById<T>(string resourceUri, string id, IDictionary<string, string> properties = null) where T : new()
        {
            const string _functionName = "GetById<T>()";
            try
            {
                var fullResourceUri = $"{resourceUri}{id}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                return await AsyncRequestHost.SendRequestAsync<T>(request, edfiClientComp, _clientName);
            }
            catch (Exception exc)
            {
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}, \r\n {exc.Message}");
                return new HttpResponse<T> { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
            }
        }

        public async Task<HttpResponse<T>> GetSingle<T>(string resourceUri, IDictionary<string, string> properties = null) where T : new()
        {
            const string _functionName = "GetSingle<T>()";
            try
            {
                var fullResourceUri = $"{resourceUri}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                return await AsyncRequestHost.SendRequestAsync<T>(request, edfiClientComp, _clientName);
            }
            catch (Exception exc)
            {
                LoggerLQ.LogQueue($"Exception in {_className} at {_functionName}, \r\n {exc.Message}");
                return new HttpResponse<T> { IsSuccess = false, StatusCode = System.Net.HttpStatusCode.BadRequest };
            }
        }

        public async Task<ServerResponse> Put(string resourceUri, string id, dynamic dto)
        {
            const string _functionName = "Put()";
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{resourceUri}{id}")
                {
                    Content = new StringContent(JsonConvert.SerializeObject(dto, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json")
                };
                return await AsyncRequestHost.SendPropagateRequestAsync(request, edfiClientComp, _clientName);
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
