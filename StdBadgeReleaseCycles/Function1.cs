using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StdBdgRCCL.Interfaces;
using StdBdgRCCL.Infrastructure;
using System.Net.Http;
using StdBdgRCCL.Infrastructure.ClientBase;
using Microsoft.Extensions.Configuration;

namespace StdBadgeReleaseCycles
{
    public class Function1
    {
        //private readonly HttpClient _edfiClient;
        //private readonly HttpClient _edfiClientComposite;
        //private readonly HttpClient _icClient;
        private IConfiguration _config;
        public Function1(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            //_edfiClient = clientFactory.CreateClient("edfiClient");
            //_icClient = clientFactory.CreateClient("icClient");
            //_edfiClientComposite = clientFactory.CreateClient("edfiClientComposite");
            _config = configuration;
        }
        [FunctionName("Function1")]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //do clients initialization when fn starts


            Updater updater = new Updater(new Athenaeum(new EdfiClientBase(), new EdfiClientCompositeBase(), new ICClientBase()),
                                            log);
            await updater.UpdateStudents();
        }
    }
}
