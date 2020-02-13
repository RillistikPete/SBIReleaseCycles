using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StdBdgRCCL.Infrastructure.Setup
{
    public class Authorization
    {
        private static readonly string edfiApiBaseUri = Environment.GetEnvironmentVariable("EdFiApiBaseUri");
        private static readonly string edfiOAuthUri = Environment.GetEnvironmentVariable("EdFiOAuthUri");
        private static readonly string apiBaseUri = Environment.GetEnvironmentVariable("ApiBaseUri");
        private static readonly string icOAuthUri = Environment.GetEnvironmentVariable("ICOAuthUri");
        private static readonly string edfiStagingUsername = Environment.GetEnvironmentVariable("EdFiStagingBasicAuthUsername");
        private static readonly string edfiStagingPass = Environment.GetEnvironmentVariable("EdFiStagingBasicAuthPassword");
        private static readonly string icClientKey = Environment.GetEnvironmentVariable("ICClientKey");
        private static readonly string icClientSecret = Environment.GetEnvironmentVariable("ICClientSecret");
        private static readonly HttpClient _accessControlClient;

        //Change ICOAuthSettings bw prod and staging
        public static async Task<Token> GetICToken()
        {
            var tokenCredentials = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", icClientKey),
                new KeyValuePair<string, string>("client_secret", icClientSecret)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiBaseUri + icOAuthUri)
            {
                Content = new FormUrlEncodedContent(tokenCredentials)
            };
            HttpResponseMessage response = await _accessControlClient.SendAsync(request);

            Token tokenResponse = new Token();

            JsonConvert.PopulateObject(response.Content.ReadAsStringAsync().Result, tokenResponse);

            return tokenResponse;
        }

        //VERSION 3X EDFI OAUTH
        public static async Task<Token> GetEdFiToken()
        {
            var grantType = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            };
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, edfiApiBaseUri + edfiOAuthUri)
            {
                Content = new FormUrlEncodedContent(grantType)
            };
            //Staging creds (change to prod)
            string userPass = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("EdFiStagingBasicAuthUsername") + ":" + Environment.GetEnvironmentVariable("EdFiStagingBasicAuthPassword")));
            request.Headers.Add("Authorization", "Basic " + userPass);
            //Staging OAuth client (change to prod)
            HttpResponseMessage response = await _accessControlClient.SendAsync(request);

            Token tokenResponse = new Token();

            JsonConvert.PopulateObject(response.Content.ReadAsStringAsync().Result, tokenResponse);

            return tokenResponse;
        }
    }
}
