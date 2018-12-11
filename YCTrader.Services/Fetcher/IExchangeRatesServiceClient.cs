using System.Threading.Tasks;

namespace YCTrader.Services.Fetcher
{
    public interface IExchangeRatesServiceClient
    {
        Task<ExchangeRate> GetCurrentExchangeRate();
    }
}