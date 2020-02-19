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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StdBdgRCCL.Infrastructure.ClientBase;
using System.Net.Http;
using Newtonsoft.Json;

[assembly: FunctionsStartup(typeof(StdBadgeReleaseCycles.Startup))]

namespace StdBadgeReleaseCycles
{
    public class Startup : FunctionsStartup
    {
        private IConfiguration _config;
        private static readonly string edfiApiBaseUri = Environment.GetEnvironmentVariable("EdFiApiBaseUri");
        private static readonly string edfiOAuthUri = Environment.GetEnvironmentVariable("EdFiOAuthUri");
        private static readonly string apiBaseUri = Environment.GetEnvironmentVariable("ApiBaseUri");

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient("edfiClient", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EdFiApiBaseUri") + Environment.GetEnvironmentVariable("EdFiBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddHttpClient("edfiClientComposite", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EdFiApiBaseUri") + Environment.GetEnvironmentVariable("EdFiCompositeBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddHttpClient("icClient", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ApiBaseUri") + Environment.GetEnvironmentVariable("ICBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddHttpClient("badgeClient", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ApiBaseUri") + Environment.GetEnvironmentVariable("BadgeBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddSingleton((s) => { return new Athenaeum(new EdfiClientBase(Authorization.edfiClient), new EdfiClientCompositeBase(Authorization.edfiClientComp),
                                                    new ICClientBase(Authorization.icClient), new BadgeClientBase(Authorization.badgeClient)); });
            builder.Services.AddScoped<IUpdater, Updater>();
            builder.Services.AddScoped<IAthenaeum, Athenaeum>();
            builder.Services.AddScoped<IEdfiClient, EdfiClientBase>();
            builder.Services.AddScoped<IEdfiClient, EdfiClientCompositeBase>();
            builder.Services.AddScoped<IICClient, ICClientBase>();

            builder.Services.AddDbContextPool<LoggingContext>(options => options.UseSqlServer(_config.GetConnectionString("LoggingDb")));
        }

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public Startup()
        {
            //parameterless ctor
        }
    }
}
