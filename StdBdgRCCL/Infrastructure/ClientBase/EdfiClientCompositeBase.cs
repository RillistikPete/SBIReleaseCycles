﻿using StdBdgRCCL.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure.ClientBase
{
    public class EdfiClientCompositeBase
    {
        public async Task<HttpResponse<List<T>>> Get<T>(string resourceUri, int offset = 0, int limit = 100, IDictionary<string, string> properties = null)
        {
            try
            {
                var fullResourceUri = $"{resourceUri}?offset={offset}&limit={limit}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                return await Clients.SendRequestForListAsync<T>(request, Clients._EdFiClientComposite, "EdFiCompositeClient");
            }
            catch (Exception ex)
            {
                Logger.Log("Get request failed in EdFi Composite Client \r\n", $"{ex}");
                var repoResponse = new RepoResponse<List<T>> { IsSuccessStatusCode = false };

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
                return await Clients.SendRequestForListAsync<T>(request, Clients._EdFiClientComposite, "EdFiClientComposite");
            }
            catch (Exception ex)
            {
                Logger.Log("Get request failed in EdFi Composite Client \r\n", $"{ex}");
                var repoResponse = new RepoResponse<List<T>> { IsSuccessStatusCode = false };

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
                return await Clients.SendRequestForListAsync<T>(request, Clients.EdFiClient, "EdFiClient");
            }
            catch (Exception ex)
            {
                Logger.Log("Get request failed in EdFi Client \r\n", $"{ex}");
                var repoResponse = new RepoResponse<List<T>> { IsSuccessStatusCode = false };

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
                return await Clients.SendRequestAsync<T>(request, Clients._EdFiClientComposite, "EdFiClientComposite");
            }
            catch (Exception ex)
            {
                Logger.Log("Get request failed in EdFi Client Composite \r\n", $"{ex}");
                var repoResponse = new RepoResponse<T> { IsSuccessStatusCode = false };

                return repoResponse;
            }
        }

        public async Task<HttpResponse<T>> GetById<T>(string resourceUri, string id, IDictionary<string, string> properties = null) where T : new()
        {
            try
            {
                var fullResourceUri = $"{resourceUri}{id}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                return await Clients.SendRequestAsync<T>(request, Clients._EdFiClientComposite, "EdFiClientComposite");
            }
            catch (Exception ex)
            {
                Logger.Log("Get request failed in EdFi Composite Client \r\n", $"{ex}");
                var repoResponse = new RepoResponse<T> { IsSuccessStatusCode = false };

                return repoResponse;
            }
        }

        public async Task<HttpResponse<T>> GetSingle<T>(string resourceUri, IDictionary<string, string> properties = null) where T : new()
        {
            try
            {
                var fullResourceUri = $"{resourceUri}";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, fullResourceUri);
                return await Clients.SendRequestAsync<T>(request, Clients._EdFiClientComposite, "EdFiClientComposite");
            }
            catch (Exception ex)
            {
                Logger.Log("Get request failed in EdFiComposite  Client \r\n", $"{ex}");
                var repoResponse = new RepoResponse<T> { IsSuccessStatusCode = false };

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
                allRecords.AddRange(fetch.Content);

                if (fetch.Content.Count == limit)
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

            HttpResponseMessage response = await Clients._EdFiClientComposite.SendAsync(request);
            return response;


        }
    }
}
