using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using StdBdgRCCL.Infrastructure;
using StdBdgRCCL.Infrastructure.ClientBase;
using StdBdgRCCL.Infrastructure.Setup;
using System.Net.Http;
using System.Threading.Tasks;

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
        //public async Task Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        public async Task Run([TimerTrigger("0 */15 6-22 * * 0-5")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            Authorization.CreateClients(_httpClientFactory);
            
            var auth = new Authorization();

            Updater updater = new Updater(new Athenaeum(new EdfiClientBase(Authorization.edfiClient), new EdfiClientCompositeBase(Authorization.edfiClientComp),
                                                        new ICClientBase(Authorization.icClient), new BadgeClientBase(Authorization.badgeClient)), log);
            await auth.FillTokens();
            await updater.UpdateStudents();
        }
    }
}
