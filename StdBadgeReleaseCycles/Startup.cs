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
using Polly;
using System.Net;
using System.Net.Http;
using System.Linq;
using Polly.Extensions.Http;

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
            }).AddPolicyHandler(RetryPolicy()).AddPolicyHandler(FallbackPolicy());
            //.AddHttpMessageHandler<ExecutingHandler>();

            builder.Services.AddHttpClient("edfiClientComposite", c =>
            {
                c.BaseAddress = new Uri(edfiApiBaseUri + edfiCompBaseUri);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddPolicyHandler(RetryPolicy()).AddPolicyHandler(FallbackPolicy());

            builder.Services.AddHttpClient("icClient", c =>
            {
                c.BaseAddress = new Uri(apiBaseUri + icBaseUri);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddPolicyHandler(RetryPolicy()).AddPolicyHandler(FallbackPolicy());

            builder.Services.AddHttpClient("badgeClient", c =>
            {
                c.BaseAddress = new Uri(apiBaseUri + badgeBaseUri);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddPolicyHandler(RetryPolicy()).AddPolicyHandler(FallbackPolicy());

            builder.Services.AddScoped((implementationFactory) =>
            {
                return new Athenaeum(new EdfiClientBase(StdBdgRCCL.Infrastructure.Setup.Authorization.edfiClient), new EdfiClientCompositeBase(StdBdgRCCL.Infrastructure.Setup.Authorization.edfiClientComp),
                 new ICClientBase(StdBdgRCCL.Infrastructure.Setup.Authorization.icClient), new BadgeClientBase(StdBdgRCCL.Infrastructure.Setup.Authorization.badgeClient));
            });
            builder.Services.AddScoped<IUpdater, Updater>();
            builder.Services.AddScoped<IAthenaeum, Athenaeum>();
            builder.Services.AddScoped<IEdfiClient, EdfiClientBase>();
            builder.Services.AddScoped<IEdfiClient, EdfiClientCompositeBase>();
            builder.Services.AddScoped<IICClient, ICClientBase>();
            builder.Services.AddDbContextPool<LoggingContext>(options => options.UseSqlServer(_config.GetConnectionString("LoggingDb")));
        }

        static IAsyncPolicy<HttpResponseMessage> RetryPolicy()
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                    .OrResult(result => statusCodesForRetry.Contains(result.StatusCode))
                    .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5) }, async (responseMessage, timeSpan, retryCount, context) =>
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        if(responseMessage.Result.RequestMessage != null)
                        {
                            var requestContent = responseMessage.Result?.RequestMessage.Content != null ? await responseMessage.Result.RequestMessage.Content.ReadAsStringAsync() : "";
                            Console.WriteLine(requestContent);
                            Console.WriteLine($"Request unsuccessful: {responseMessage.Result.RequestMessage.RequestUri} \r\n" +
                                                $"Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                        }
                        else
                        {
                            Console.WriteLine($"Request unsuccessful: {responseMessage.Result.ReasonPhrase} \r\n" +
                                                $"Waiting {timeSpan} before next retry. Retry attempt {retryCount}", ConsoleColor.Blue);
                        }
                        var responseContent = await responseMessage.Result.Content.ReadAsStringAsync();
                        Console.WriteLine(responseContent);
                        Console.ResetColor();
                    });
        }

        static IAsyncPolicy<HttpResponseMessage> FallbackPolicy()
        {
            return HttpPolicyExtensions.HandleTransientHttpError()
                    .OrResult(result => statusCodesForRetry.Contains(result.StatusCode))
                    .FallbackAsync(async b =>
                    {
                        return new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Content = new StringContent("Fallback policy used")
                        };
                    });
        }


        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public Startup()
        {
        }

        public static HttpStatusCode[] statusCodesForRetry =
        {
            HttpStatusCode.RequestTimeout, // 408
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout, // 504
            HttpStatusCode.BadRequest,
            HttpStatusCode.MethodNotAllowed,
            HttpStatusCode.NoContent,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.NotFound,
        };

    }
}
