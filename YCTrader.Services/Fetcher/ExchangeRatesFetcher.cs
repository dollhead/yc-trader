using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using YCTrader.Services.Storage;

namespace YCTrader.Services.Fetcher
{
    public class ExchangeRatesFetcher : IHostedService, IDisposable
    {
        private readonly IExchangeRatesStorage _exchangeRatesStorage;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<ExchangeRatesFetcherOptions> _options;
        private Timer _timer;

        public ExchangeRatesFetcher(ILogger<ExchangeRatesFetcher> logger,
            IOptionsMonitor<ExchangeRatesFetcherOptions> options,
            IExchangeRatesStorage exchangeRatesStorage)
        {
            _exchangeRatesStorage = exchangeRatesStorage;
            _options = options;
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Exchange rates fetcher is starting.");

            await GenerateDataForPreviousDays();
            
            _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_options.CurrentValue.FetchInterval));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Exchange rates fetcher is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
            var exchangeRate = await GetCurrentExchangeRate();
            _exchangeRatesStorage.SaveExchangeRate(exchangeRate);
        }

        private async Task<ExchangeRate> GetCurrentExchangeRate()
        {
            using (var response = await _httpClient.GetAsync(_options.CurrentValue.ExchangeRatesServiceApiUrl))
            {
                var contentString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<ExchangeRate>>(contentString).FirstOrDefault();
            }
        }
        
        private async Task GenerateDataForPreviousDays()
        {
            var currentExchangeRate = await GetCurrentExchangeRate();
            var numberOfPreviousDaysForDataGeneration = _options.CurrentValue.NumberOfPreviousDaysToFetch;
            var todayDate = DateTime.Today;
            var startDate = todayDate.Subtract(TimeSpan.FromDays(numberOfPreviousDaysForDataGeneration));

            var currentDate = startDate;
            var random = new Random();

            var maxExchangeRateMultiplier = _options.CurrentValue.MaxExchangeRateMultiplier;
            var minExchangeRateMultiplier = _options.CurrentValue.MinExchangeRateMultiplier;
            while (currentDate != todayDate)
            {
                var randomRateMultiplier = (decimal)(random.NextDouble() * (maxExchangeRateMultiplier - minExchangeRateMultiplier) + minExchangeRateMultiplier);
                
                _exchangeRatesStorage.SaveExchangeRate(new ExchangeRate
                {
                    Price = currentExchangeRate.Price * randomRateMultiplier,
                    Timestamp = new DateTimeOffset(currentDate).ToUnixTimeSeconds()
                });
                
                currentDate = currentDate.AddSeconds(_options.CurrentValue.FetchInterval);
            }
        }
    }
}