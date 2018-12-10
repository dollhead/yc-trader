using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace YCTrader.Services
{
    public class ExchangeRatesStorage : IExchangeRatesStorage
    {
        private readonly ConcurrentDictionary<DateTime, decimal> _exchangeRatesForCurrentDay =
            new ConcurrentDictionary<DateTime, decimal>();

        public IEnumerable<decimal> GetExchangeRatesForCurrentDay()
        {
            return _exchangeRatesForCurrentDay.Values;
        }

        public void SaveExchangeRate(ExchangeRate exchangeRate)
        {
            var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(exchangeRate.Timestamp)).DateTime;
            var price = decimal.Parse(exchangeRate.Price);

            _exchangeRatesForCurrentDay[dateTime] = price;
        }

        public decimal GetLatestExchangeRate()
        {
            return _exchangeRatesForCurrentDay.Last().Value;
        }
    }
}