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
        [FunctionName("Function1")]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            Updater updater = new Updater(new Athenaeum(new EdfiClientBase(), new EdfiClientCompositeBase(), new ICClientBase()),
                                            log);



            await updater.UpdateStudents();
        }
    }
}
