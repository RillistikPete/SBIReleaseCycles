using Newtonsoft.Json;
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
    public class ICClientBase : IICClient
    {
        private readonly HttpClient _icClient;
        private const string _clientName = "ICClient";
        public ICClientBase(HttpClient client)
        {
            _icClient = client;
        }

        public async Task<HttpResponse<List<T>>> Get<T>(string resourceUri, int offset = 0, int pagesize = 1000, IDictionary<string, string> properties = null)
        {
            var fullResourceUri = $"{resourceUri}?offset={offset}&pagesize={pagesize}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
            return await AsyncRequestHost.SendRequestForListAsync<T>(request, _icClient, _clientName);
        }

        public async Task<HttpResponse<List<T>>> GetById<T>(string resourceUri, string id, IDictionary<string, string> properties = null)
        {
            var fullResourceUri = $"{resourceUri}{id}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
            return await AsyncRequestHost.SendRequestForListAsync<T>(request, _icClient, _clientName);
        }

        public async Task<HttpResponse<List<T>>> GetByExample<T>(string resourceUri, IDictionary<string, string> properties = null, int offset = 0, int pagesize = 100)
        {
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
                LoggerLQ.LogQueue($"Get request failed in IC Client Base at GetByExample<T>, {_clientName} \r\n {ex.Message}");
                var repoResponse = new HttpResponse<List<T>> { IsSuccess= false };

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
                return await AsyncRequestHost.SendRequestAsync<T>(request, _icClient, _clientName);
            }
            catch (Exception ex)
            {
                LoggerLQ.LogQueue($"Get request failed in IC Client Base at GetSingleByExample<T>, {_clientName} \r\n {ex.Message}");
                var repoResponse = new HttpResponse<T> { IsSuccess = false };

                return repoResponse;
            }
        }


        public async Task<List<T>> GetAll<T>(string resourceUri, int offset = 0, int pagesize = 1000, IDictionary<string, string> properties = null)
        {
            List<T> allRecords = new List<T>();
            bool isFinished = false;
            do
            {
                var fetch = await Get<T>(resourceUri, offset, pagesize);

                if (fetch.ResponseContent.Count == pagesize + 1)
                {
                    fetch.ResponseContent.RemoveAt(pagesize);
                    allRecords.AddRange(fetch.ResponseContent);
                    offset += pagesize;
                }
                else
                {
                    allRecords.AddRange(fetch.ResponseContent);
                    isFinished = true;
                }
            } while (!isFinished);

            return allRecords;
        }

        public async Task<ServerResponse> Put(string resourceUri, string id, dynamic dto, IDictionary<string, string> properties = null)
        {
            var json = JsonConvert.SerializeObject(dto, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{resourceUri}{id}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };

            return await AsyncRequestHost.SendPropagateRequestAsync(request, _icClient, _clientName);
        }

        public async Task<ServerResponse> Delete(string resourceUri, string id, string checksum, IDictionary<string, string> properties = null)
        {
            var fullResourceUri = $"{resourceUri}{id}?checksum={checksum}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, fullResourceUri);
            ServerResponse serverResponse = new ServerResponse();

            return await AsyncRequestHost.SendPropagateRequestAsync(request, _icClient, _clientName);

        }

        public async Task<ServerResponse> Post(string resourceUri, dynamic dto, IDictionary<string, string> properties = null)
        {
            var fullResourceUri = $"{resourceUri}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, fullResourceUri)
            {
                Content = new StringContent(JsonConvert.SerializeObject(dto, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8, "application/json")
            };
            ServerResponse serverResponse = new ServerResponse();

            return await AsyncRequestHost.SendPropagateRequestAsync(request, _icClient, _clientName);
        }
    }
}
