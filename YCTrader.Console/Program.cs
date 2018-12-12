using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using YCTrader.Services.Fetcher;
using YCTrader.Services.Predictor;
using YCTrader.Services.Storage;

namespace YCTrader.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables().Build();

            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .ConfigureServices(ConfigureServices)
                .Configure(app =>
                {
                    app.UseRouter(r => r.MapGet("prediction",
                        async (request, response, routeData) => { await GetExchangeRateStatus(app, response); }));

                    app.UseHttpsRedirection();
                })
                .Build();

            webHost.Run();
        }

        private static void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddHostedService<ExchangeRatesFetcher>();
            services.AddSingleton<IExchangeRatesStorage, ExchangeRatesStorage>();
            services.AddTransient<IExchangeRatePredictor, ExchangeRatePredictor>();

            var fetcherOptionsSection = context.Configuration.GetSection("exchangeRatesFetcherOptions");
            var fetcherOptions = fetcherOptionsSection.Get<ExchangeRatesFetcherOptions>();

            services.AddHttpClient<IExchangeRatesServiceClient, ExchangeRatesServiceClient>()
                .AddTransientHttpErrorPolicy(p => p.RetryAsync(fetcherOptions.NumberOfRetries));

            services.Configure<ExchangeRatesFetcherOptions>(fetcherOptionsSection);
            services.Configure<ExchangeRatePredictorOptions>(
                context.Configuration.GetSection("exchangeRatePredictorOptions"));

            services.AddRouting();
        }

        private static async Task GetExchangeRateStatus(IApplicationBuilder app, HttpResponse response)
        {
            var predictor = app.ApplicationServices.GetService<IExchangeRatePredictor>();
            var rateTrend = predictor.GetExchangeRateTrend();

            var exchangeRateProvider = app.ApplicationServices.GetService<IExchangeRatesStorage>();
            var currentRate = exchangeRateProvider.GetLatestExchangeRate();

            response.ContentType = "application/json";
            response.StatusCode = StatusCodes.Status200OK;
            var responseJson =
                JsonConvert.SerializeObject(ExchangeRateStatus.ToExchangeRateStatus(currentRate, rateTrend));

            await response.WriteAsync(responseJson);
        }
    }
}