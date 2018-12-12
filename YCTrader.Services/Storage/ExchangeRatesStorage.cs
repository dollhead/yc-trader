using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace YCTrader.Services.Storage
{
    public class ExchangeRatesStorage : IExchangeRatesStorage
    {
        private readonly object _currentExchangeRateUpdateLock = new object();
        private readonly ConcurrentDictionary<long, decimal> _exchangeRatesForCurrentDay = new ConcurrentDictionary<long, decimal>();
        private readonly List<decimal> _closingPricesForPreviousDays = new List<decimal>();
        
        private ExchangeRate _latestExchangeRate;
        
        public ExchangeRate LatestExchangeRate
        {
            get => _latestExchangeRate;
            set
            {
                lock (_currentExchangeRateUpdateLock)
                {
                    if (_latestExchangeRate == null || 
                        value.Timestamp > _latestExchangeRate.Timestamp)
                    {
                        _latestExchangeRate = value;
                    }
                }
            }
        }

        public void SaveExchangeRate(ExchangeRate exchangeRate)
        {
            var dateTime = exchangeRate.Timestamp;
            var price = exchangeRate.Price;

            _exchangeRatesForCurrentDay[dateTime] = price;
            
            LatestExchangeRate = exchangeRate;
        }

        public void SaveClosingPrices(IList<decimal> closingPrices)
        {
            _closingPricesForPreviousDays.AddRange(closingPrices);
        }

        public IDictionary<long, decimal> GetExchangeRatesForCurrentDay()
        {
            return _exchangeRatesForCurrentDay
                .Where(x => DateTimeOffset.FromUnixTimeSeconds(x.Key).Date == DateTime.UtcNow.Date)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public IEnumerable<decimal> GetClosingPricesForPreviousDays(int numberOfDays)
        {
            return _closingPricesForPreviousDays.TakeLast(numberOfDays);
        }
    }
}