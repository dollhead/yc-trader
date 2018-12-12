using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace YCTrader.Services.Storage
{
    public interface IExchangeRatesStorage
    {
        void SaveExchangeRate(ExchangeRate exchangeRate);

        IDictionary<long, decimal> GetExchangeRatesForCurrentDay();

        ExchangeRate LatestExchangeRate { get; }

        IEnumerable<decimal> GetClosingPricesForPreviousDays(int numberOfDays);
        void SaveClosingPrices(IList<decimal> closingPrices);
    }
}