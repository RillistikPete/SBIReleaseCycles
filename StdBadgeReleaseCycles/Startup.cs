using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StdBdgRCCL;
using StdBdgRCCL.Infrastructure;
using StdBdgRCCL.Infrastructure.ClientBase;
using StdBdgRCCL.Infrastructure.Setup;
using StdBdgRCCL.Interfaces;
using System;

[assembly: FunctionsStartup(typeof(StdBadgeReleaseCycles.Startup))]

namespace StdBadgeReleaseCycles
{
    public class Startup : FunctionsStartup
    {
        private IConfiguration _config;
        private static readonly string edfiApiBaseUri = Environment.GetEnvironmentVariable("EdFiApiBaseUri");
        private static readonly string edfiBaseUri = Environment.GetEnvironmentVariable("EdFiBaseUri");
        private static readonly string edfiCompBaseUri = Environment.GetEnvironmentVariable("EdFiCompositeBaseUri");
        private static readonly string edfiOAuthUri = Environment.GetEnvironmentVariable("EdFiOAuthUri");
        private static readonly string icBaseUri = Environment.GetEnvironmentVariable("ICBaseUri");
        private static readonly string badgeBaseUri = Environment.GetEnvironmentVariable("BadgeBaseUri");
        private static readonly string apiBaseUri = Environment.GetEnvironmentVariable("ApiBaseUri");

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient("edfiClient", c =>
            {
                c.BaseAddress = new Uri(edfiApiBaseUri + edfiBaseUri);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddHttpClient("edfiClientComposite", c =>
            {
                c.BaseAddress = new Uri(edfiApiBaseUri + edfiCompBaseUri);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddHttpClient("icClient", c =>
            {
                c.BaseAddress = new Uri(apiBaseUri + icBaseUri);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddHttpClient("badgeClient", c =>
            {
                c.BaseAddress = new Uri(apiBaseUri + badgeBaseUri);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            builder.Services.AddScoped((implementationFactory) =>
            {
                return new Athenaeum(new EdfiClientBase(Authorization.edfiClient), new EdfiClientCompositeBase(Authorization.edfiClientComp),
                 new ICClientBase(Authorization.icClient), new BadgeClientBase(Authorization.badgeClient));
            });
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
        }

        //var registry = services.AddPolicyRegistry();

        //var timeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
        //var longTimeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));

        //registry.Add("regular", timeout);
        //    registry.Add("long", longTimeout);


        // Use a specific named policy from the registry. Simplest way, policy is cached for the
            // lifetime of the handler.
        //    .AddPolicyHandlerFromRegistry("regular")

        //    // Run some code to select a policy based on the request
        //    .AddPolicyHandler((request) =>
        //    {
        //    return request.Method == HttpMethod.Get ? timeout : longTimeout;
        //})

        //    // Run some code to select a policy from the registry based on the request
        //    .AddPolicyHandlerFromRegistry((reg, request) =>
        //    {
        //    return request.Method == HttpMethod.Get ?
        //        reg.Get<IAsyncPolicy<HttpResponseMessage>>("regular") :
        //        reg.Get<IAsyncPolicy<HttpResponseMessage>>("long");
        //})
            
        //    // Build a policy that will handle exceptions, 408s, and 500s from the remote server
        //    .AddTransientHttpErrorPolicy(p => p.RetryAsync())

        //    .AddHttpMessageHandler(() => new RetryHandler()) // Retry requests to github using our retry handler
        //    .AddTypedClient<GitHubClient>();


    }
}
