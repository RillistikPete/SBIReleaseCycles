using StdBdgRCCL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Interfaces
{
    public interface IICClient
    {
        /// <summary>
        /// Send a GET request
        /// </summary>
        /// <typeparam name="T">Resource Model</typeparam>
        /// <param name="resourceUri">Resource Endpoint URI</param>
        /// <param name="offset">Number of records to skip</param>
        /// <param name="pagesize">Record limit</param>
        /// <param name="properties">Properties to pass along for development purposes</param>
        /// <returns>List of records with model specified in Type</returns>
        Task<HttpResponse<List<T>>> Get<T>(string resourceUri, int offset = 0, int pagesize = 1000, IDictionary<string, string> properties = null);
        /// <summary>
        /// Send a GET request for a specific record
        /// </summary>
        /// <typeparam name="T">Resource Model</typeparam>
        /// <param name="resourceUri">Resource Endpoint URI (End with a "/")</param>
        /// <param name="id">Record Id</param>
        /// <param name="properties">Properties to pass along for development purposes</param>
        /// <returns></returns>
        Task<HttpResponse<List<T>>> GetById<T>(string resourceUri, string id, IDictionary<string, string> properties = null);
        /// <summary>
        /// Send a GET request with query search parameters
        /// </summary>
        /// <typeparam name="T">Resource Model</typeparam>
        /// <param name="resourceUri">Resource Endpoint URI including the query parameters (ex: "endpoint?customId=123")</param>
        /// <param name="properties">Properties to pass along for development purposes</param>
        /// <param name="offset">Number of records to skip</param>
        /// <param name="pagesize">Records pagesize</param>
        /// <returns>List of records with model specified in Type</returns>
        Task<HttpResponse<List<T>>> GetByExample<T>(string resourceUri, IDictionary<string, string> properties = null, int offset = 0, int pagesize = 100);
        /// <summary>
        /// Send a GET request and get all available records.
        /// </summary>
        /// <typeparam name="T">Resource Model</typeparam>
        /// <param name="resourceUri">Resource Endpoint URI</param>
        /// <param name="offset">Number of records to skip</param>
        /// <param name="pagesize">Record limit</param>
        /// <param name="properties">Properties to pass along for development purposes</param>
        /// <returns>List of records with model specified in Type</returns>
        Task<List<T>> GetAll<T>(string resourceUri, int offset = 0, int pagesize = 1000, IDictionary<string, string> properties = null);

        //===================================
        /// <summary>
        /// Send a PUT request
        /// </summary>
        /// <param name="resourceUri">Resource Endpoint URI</param>
        /// <param name="id">Record Id to send request to</param>
        /// <param name="dto">Data Transfer Object to send with the request</param>
        /// <param name="properties">Properties to pass along for development purposes</param>
        /// <returns></returns>
        Task<ServerResponse> Put(string resourceUri, string id, dynamic dto, IDictionary<string, string> properties = null);
        /// <summary>
        /// Send a DELETE request
        /// </summary>
        /// <param name="resourceUri">Resource Endpoint URI (End with a "/")</param>
        /// <param name="id">Record Id to send request to</param>
        /// <param name="checksum">Checksum required by api</param>
        /// <param name="properties">Properties to pass along for development purposes</param>
        /// <returns></returns>
        Task<ServerResponse> Delete(string resourceUri, string id, string checksum, IDictionary<string, string> properties = null);
        /// <summary>
        /// Send a POST request
        /// </summary>
        /// <param name="resourceUri">Resource Endpoint URI (End with a "/")</param>
        /// <param name="dto">Data to send with request</param>
        /// <param name="properties">Properties to pass along for development purposes</param>
        /// <returns></returns>
        Task<ServerResponse> Post(string resourceUri, dynamic dto, IDictionary<string, string> properties = null);

    }
}
