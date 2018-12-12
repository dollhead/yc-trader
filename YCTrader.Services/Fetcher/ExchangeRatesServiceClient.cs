using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace YCTrader.Services.Fetcher
{
    public class ExchangeRatesServiceClient : IExchangeRatesServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<ExchangeRatesFetcherOptions> _options;

        public ExchangeRatesServiceClient(HttpClient httpClient, IOptions<ExchangeRatesFetcherOptions> options)
        {
            _httpClient = httpClient;
            _options = options;
        }

        public async Task<ExchangeRate> GetCurrentExchangeRate()
        {
            using (var response = await _httpClient.GetAsync(_options.Value.ExchangeRatesServiceApiUrl))
            {
                var contentString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<ExchangeRate>>(contentString).FirstOrDefault();
            }
        }
    }
}