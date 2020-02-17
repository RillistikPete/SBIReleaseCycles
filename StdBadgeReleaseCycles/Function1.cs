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
using StdBdgRCCL.Infrastructure.Setup;

namespace StdBadgeReleaseCycles
{
    public class Function1
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public Function1(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }
        [FunctionName("Function1")]
        public async Task Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var auth = new Authorization();
            Authorization.CreateClients(_httpClientFactory);

            Updater updater = new Updater(new Athenaeum(new EdfiClientBase(Authorization.edfiClient), new EdfiClientCompositeBase(Authorization.edfiClientComp),
                                                        new ICClientBase(Authorization.icClient), new BadgeClientBase(Authorization.badgeClient)), log);
            await auth.FillTokens();
            await updater.UpdateStudents();
        }
    }
}
