using Newtonsoft.Json;
using StdBdgRCCL.Infrastructure.Setup;
using StdBdgRCCL.Interfaces;
using StdBdgRCCL.Models;
using StdBdgRCCL.Models.AzureDb;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure.ClientBase
{
    public class EdfiClientBase : IEdfiClient
    {
        private const string _clientName = "EdFiClient";
        public static HttpClient edfiClient;
        public EdfiClientBase(HttpClient client)
        {
            edfiClient = client;
        }
        public async Task<HttpResponse<List<T>>> Get<T>(string resourceUri, int offset = 0, int limit = 100, IDictionary<string, string> properties = null)
        {
            try
            {
                var fullResourceUri = $"{resourceUri}?offset={offset}&limit={limit}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                return await AsyncRequestHost.SendRequestForListAsync<T>(request, edfiClient, _clientName);
            }
            catch (Exception ex)
            {
                LoggerLQ.LogQueue($"Get request failed in EdFi Client Base at Get<T>, {_clientName} \r\n {ex.Message}");
                var repoResponse = new HttpResponse<List<T>> { IsSuccess = false };

                return repoResponse;
            }
        }

        public async Task<HttpResponse<List<T>>> GetByExample<T>(string resourceUri, IDictionary<string, string> properties = null, int offset = 0, int limit = 100)
        {
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
                return await AsyncRequestHost.SendRequestForListAsync<T>(request, edfiClient, _clientName);
            }
            catch (Exception ex)
            {
                LoggerLQ.LogQueue($"Get request failed in EdFi Client Base at GetByExample<T>, {_clientName} \r\n {ex.Message}");
                var repoResponse = new HttpResponse<List<T>> { IsSuccess = false };

                return repoResponse;
            }
        }

        public async Task<HttpResponse<List<T>>> GetByExampleLowerLimit<T>(string resourceUri, IDictionary<string, string> properties = null, int offset = 0, int limit = 25)
        {
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
                return await AsyncRequestHost.SendRequestForListAsync<T>(request, edfiClient, _clientName);
            }
            catch (Exception ex)
            {
                LoggerLQ.LogQueue($"Get request failed in EdFi Client Base at GetByExampleLowerLimit<T>, {_clientName} \r\n {ex.Message}");
                var repoResponse = new HttpResponse<List<T>> { IsSuccess = false };

                return repoResponse;
            }
        }

        public async Task<HttpResponse<T>> GetSingleByExample<T>(string resourceUri, IDictionary<string, string> properties = null) where T : new()
        {
            try
            {
                var fullResourceUri = $"{resourceUri}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                if (properties != null)
                {
                    foreach (var prop in properties)
                        request.Properties.Add(prop.Key, prop.Value);
                }
                return await AsyncRequestHost.SendRequestAsync<T>(request, edfiClient, _clientName);
            }
            catch (Exception ex)
            {
                LoggerLQ.LogQueue($"Get request failed in EdFi Client Base at GetSingleByExample<T>, {_clientName} \r\n {ex.Message}");
                var repoResponse = new HttpResponse<T> { IsSuccess = false };

                return repoResponse;
            }
        }

        public async Task<HttpResponse<T>> GetById<T>(string resourceUri, string id, IDictionary<string, string> properties = null) where T : new()
        {
            try
            {
                var fullResourceUri = $"{resourceUri}{id}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                return await AsyncRequestHost.SendRequestAsync<T>(request, edfiClient, _clientName);
            }
            catch (Exception ex)
            {
                LoggerLQ.LogQueue($"Get request failed in EdFi Client Base at GetById<T>, {_clientName} \r\n {ex.Message}");
                var repoResponse = new HttpResponse<T> { IsSuccess = false };

                return repoResponse;
            }
        }

        public async Task<HttpResponse<T>> GetSingle<T>(string resourceUri, IDictionary<string, string> properties = null) where T : new()
        {
            try
            {
                var fullResourceUri = $"{resourceUri}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                return await AsyncRequestHost.SendRequestAsync<T>(request, edfiClient, _clientName);
            }
            catch (Exception ex)
            {
                LoggerLQ.LogQueue($"Get request failed in EdFi Client Base at GetSingle<T>, {_clientName} \r\n {ex.Message}");
                var repoResponse = new HttpResponse<T>{ IsSuccess = false };

                return repoResponse;
            }
        }

        public async Task<List<T>> GetAll<T>(string resourceUri, int offset = 0, int limit = 100, IDictionary<string, string> properties = null)
        {
            List<T> allRecords = new List<T>();
            bool isFinished = false;
            do
            {
                var fetch = await Get<T>(resourceUri, offset, limit);
                allRecords.AddRange(fetch.ResponseContent);

                if (fetch.ResponseContent.Count == limit)
                {
                    offset += limit;
                }
                else
                {
                    isFinished = true;
                }
            } while (!isFinished);


            return allRecords;
        }

        public async Task<HttpResponseMessage> Put(string resourceUri, string id, dynamic dto)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{resourceUri}{id}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(dto, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await edfiClient.SendAsync(request);
            return response;


        }
    }
}
