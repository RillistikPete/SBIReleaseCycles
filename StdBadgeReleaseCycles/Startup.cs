using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using StdBdgRCCL.Infrastructure;
using StdBdgRCCL.Interfaces;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using StdBdgRCCL.Infrastructure.Setup;

[assembly: FunctionsStartup(typeof(StdBadgeReleaseCycles.Startup))]

namespace StdBadgeReleaseCycles
{

    public class Startup : FunctionsStartup
    {
        private static string _edfiToken { get; set; } = "";
        private static long _edfiTokenExpiration { get; set; }
        private static string _icToken { get; set; } = "";
        private static long _icTokenExpiration { get; set; }
        //private readonly CancellationToken _cancellationToken;

        public static async Task FillTokens()
        {
            var token = await Authorization.GetEdFiToken();
            _edfiToken = token.AccessToken;
            _edfiTokenExpiration = token.ExpiresIn;

            var icToken = await Authorization.GetICToken();
            _icToken = icToken.AccessToken;
            _icTokenExpiration = icToken.ExpiresIn;
            return;
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient("edfiClient", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EdFiApiBaseUri") + Environment.GetEnvironmentVariable("EdFiBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _edfiToken);
            });

            builder.Services.AddHttpClient("edfiClientComposite", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EdFiApiBaseUri") + Environment.GetEnvironmentVariable("EdFiV3CompositeBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _edfiToken);
            });
            builder.Services.AddHttpClient("icClient", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ApiBaseUri") + Environment.GetEnvironmentVariable("ICBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _icToken);
            });
            builder.Services.AddSingleton((s)=> { return new Athenaeum(); });
            builder.Services.AddScoped<IUpdater, Updater>();
        }
        //public void Configure(IWebJobsBuilder builder)
        //{

        //}
    }
}
