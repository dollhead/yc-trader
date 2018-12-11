using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace YCTrader.Services.Storage
{
    public interface IExchangeRatesStorage
    {
        void SaveExchangeRate(ExchangeRate exchangeRate);

        IDictionary<long, decimal> GetExchangeRatesForCurrentDay();

        decimal GetLatestExchangeRate();

        IDictionary<long, decimal> GetExchangeRatesForPreviousDays(int numberOfDays);
    }
}