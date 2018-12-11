using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace YCTrader.Services.Storage
{
    public class ExchangeRatesStorage : IExchangeRatesStorage
    {
        private readonly ConcurrentDictionary<long, decimal> _exchangeRates =
            new ConcurrentDictionary<long, decimal>();

        public void SaveExchangeRate(ExchangeRate exchangeRate)
        {
            var dateTime = exchangeRate.Timestamp;
            var price = exchangeRate.Price;

            _exchangeRates[dateTime] = price;
        }

        public decimal GetLatestExchangeRate()
        {
            return _exchangeRates.Last().Value;
        }

        public IDictionary<long, decimal> GetExchangeRatesForCurrentDay()
        {
            return _exchangeRates
                .Where(x => DateTimeOffset.FromUnixTimeSeconds(x.Key).Date == DateTime.UtcNow.Date)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public IDictionary<long, decimal> GetExchangeRatesForPreviousDays(int numberOfDays)
        {
            return _exchangeRates
                .Where(x => DateTimeOffset.FromUnixTimeSeconds(x.Key).DateTime > DateTime.UtcNow.Subtract(TimeSpan.FromDays(numberOfDays)))
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}