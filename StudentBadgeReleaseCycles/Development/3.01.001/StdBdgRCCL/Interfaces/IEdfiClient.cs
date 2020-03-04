using StdBdgRCCL.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Interfaces
{
    public interface IEdfiClient
    {
        /// <summary>
        /// Send a GET request
        /// </summary>
        /// <typeparam name="T">Resource Model</typeparam>
        /// <param name="resourceUri">Resource Endpoint URI</param>
        /// <param name="offset">Number of records to skip</param>
        /// <param name="limit">Record limit</param>
        /// <returns>List of records with model specified in Type</returns>
        Task<HttpResponse<List<T>>> Get<T>(string resourceUri, int offset = 0, int limit = 100, IDictionary<string, string> properties = null);

        /// <summary>
        /// Send a GET request with query search parameters
        /// </summary>
        /// <typeparam name="T">Resource Model</typeparam>
        /// <param name="resourceUri">Resource Endpoint URI including the query parameters (ex: "endpoint?customId=123")</param>
        /// <param name="offset">Number of records to skip</param>
        /// <param name="limit">Record limit</param>
        /// <returns>List of records with model specified in Type</returns>
        Task<HttpResponse<List<T>>> GetByExample<T>(string resourceUri, IDictionary<string, string> properties = null, int offset = 0, int limit = 100);

        /// <summary>
        /// Send a GET request with query search parameters
        /// </summary>
        /// <typeparam name="T">Resource Model</typeparam>
        /// <param name="resourceUri">Resource Endpoint URI including the query parameters</param>
        /// <param name="offset">Number of records to skip</param>
        /// <param name="limit">Record limit</param>
        /// <returns>List of records with model specified in Type</returns>
        Task<HttpResponse<List<T>>> GetByExampleLowerLimit<T>(string resourceUri, IDictionary<string, string> properties = null, int offset = 0, int limit = 25);

        /// <summary>
        /// Send a GET request with query search parameters for a single json return object
        /// </summary>
        /// <typeparam name="T">Resource Model</typeparam>
        /// <param name="resourceUri">Resource Endpoint URI including the query parameters (ex: "endpoint?customId=123")</param>
        /// <returns>A single record with model specified in Type</returns>
        Task<HttpResponse<T>> GetSingleByExample<T>(string resourceUri, IDictionary<string, string> properties = null) where T : new();

        /// <summary>
        /// Send a GET request for a specific record by guid id
        /// </summary>
        /// <typeparam name="T">Resource Model</typeparam>
        /// <param name="resourceUri">Resource Endpoint URI</param>
        /// <param name="id">Record Id</param>
        /// <returns></returns>
        Task<HttpResponse<T>> GetById<T>(string resourceUri, string id, IDictionary<string, string> properties = null) where T : new();

        /// <summary>
        /// Send a GET request that should only return one record
        /// </summary>
        /// <typeparam name="T">Resource Model</typeparam>
        /// <param name="resourceUri">Resource Endpoint URI</param>
        /// <returns>Single Record Object specified by <typeparamref name="T"/></returns>
        Task<HttpResponse<T>> GetSingle<T>(string resourceUri, IDictionary<string, string> properties = null) where T : new();

        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="resourceUri">Resource Endpoint URI</param>
        /// <param name="id">Record Id to send request to</param>
        /// <param name="dto">Data Transfer Object to send with the request</param>
        /// <returns></returns>
        Task<ServerResponse> Put(string resourceUri, string id, dynamic dto);
    }
}
