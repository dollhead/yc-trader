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

namespace YCTrader.Services
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Exchange rates fetcher is starting.");

            _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_options.CurrentValue.FetchInterval));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Exchange rates fetcher is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state)
        {
            using (var response = await _httpClient.GetAsync(_options.CurrentValue.ExchangeRatesServiceApiUrl))
            {
                var contentString = await response.Content.ReadAsStringAsync();
                var exchangeRate = JsonConvert.DeserializeObject<IEnumerable<ExchangeRate>>(contentString).FirstOrDefault();

                _exchangeRatesStorage.SaveExchangeRate(exchangeRate);
            }
        }
    }
}