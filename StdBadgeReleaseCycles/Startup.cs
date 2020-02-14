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

[assembly: FunctionsStartup(typeof(StdBadgeReleaseCycles.Startup))]

namespace StdBadgeReleaseCycles
{
    public class Startup : FunctionsStartup
    {
        private IConfiguration _config;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient<EdfiClientBase>("edfiClient", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EdFiApiBaseUri") + Environment.GetEnvironmentVariable("EdFiBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.
            });

            builder.Services.AddHttpClient<EdfiClientCompositeBase>("edfiClientComposite", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EdFiApiBaseUri") + Environment.GetEnvironmentVariable("EdFiV3CompositeBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _edfiToken);
            });

            builder.Services.AddHttpClient<ICClientBase>("icClient", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ApiBaseUri") + Environment.GetEnvironmentVariable("ICBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _icToken);
            });
            
            builder.Services.AddSingleton((s) => { return new Athenaeum(new EdfiClientBase(), new EdfiClientCompositeBase(), new ICClientBase()); });
            builder.Services.AddScoped<IUpdater, Updater>();
            builder.Services.AddScoped<IAthenaeum, Athenaeum>();
            builder.Services.AddScoped<IEdfiClient, EdfiClientBase>();
            builder.Services.AddScoped<IEdfiClient, EdfiClientCompositeBase>();
            builder.Services.AddScoped<IICClient, ICClientBase>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<LoggingContext>(options => options.UseSqlServer(_config.GetConnectionString("LoggingDb")));
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
