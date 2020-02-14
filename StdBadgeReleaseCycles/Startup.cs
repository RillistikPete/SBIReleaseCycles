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
        private static string _edfiToken { get; set; } = "";
        private static long _edfiTokenExpiration { get; set; }
        private static string _icToken { get; set; } = "";
        private static long _icTokenExpiration { get; set; }
        private IConfiguration _config;
        //private readonly CancellationToken _cancellationToken;

        public override void Configure(IFunctionsHostBuilder builder)
        {

            //var config = new ConfigurationBuilder()
            //   .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            //   .AddEnvironmentVariables()
            //   .Build();

            //builder.Services.AddSingleton(config);

            builder.Services.AddHttpClient<EdfiClientBase>("edfiClient", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EdFiApiBaseUri") + Environment.GetEnvironmentVariable("EdFiBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                //c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _edfiToken);
            });

            builder.Services.AddHttpClient<EdfiClientCompositeBase>("edfiClientComposite", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("EdFiApiBaseUri") + Environment.GetEnvironmentVariable("EdFiV3CompositeBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                //c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _edfiToken);
            });

            builder.Services.AddHttpClient<ICClientBase>("icClient", c =>
            {
                c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ApiBaseUri") + Environment.GetEnvironmentVariable("ICBaseUri"));
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                //c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _icToken);
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

        public Startup()
        {
            //parameterless ctor
        }


        public void Configure(IWebJobsBuilder builder)
        {

        }
    }
}
