using System.Collections.Generic;

namespace YCTrader.Services
{
    public interface IExchangeRatesStorage
    {
        void SaveExchangeRate(ExchangeRate exchangeRate);
         
        IEnumerable<decimal> GetExchangeRatesForCurrentDay();
        
        decimal GetLatestExchangeRate();
    }
}