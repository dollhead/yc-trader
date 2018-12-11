using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using YCTrader.Services.Fetcher;
using YCTrader.Services.Predictor;
using YCTrader.Services.Storage;

namespace YCTrader.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHostedService<ExchangeRatesFetcher>();
            services.AddSingleton<IExchangeRatesStorage, ExchangeRatesStorage>();
            services.AddTransient<IExchangeRatePredictor, ExchangeRatePredictor>();

            var fetcherOptionsSection = Configuration.GetSection("exchangeRatesFetcherOptions");
            var fetcherOptions = fetcherOptionsSection.Get<ExchangeRatesFetcherOptions>();
            
            services.AddHttpClient<IExchangeRatesServiceClient, ExchangeRatesServiceClient>()
                .AddTransientHttpErrorPolicy(p => p.RetryAsync(fetcherOptions.NumberOfRetries));

            services.Configure<ExchangeRatesFetcherOptions>(fetcherOptionsSection);
            services.Configure<ExchangeRatePredictorOptions>(Configuration.GetSection("exchangeRatePredictorOptions"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}