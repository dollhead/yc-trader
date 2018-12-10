namespace YCTrader.Services
{
    public interface IExchangeRatesStorage
    {
        void SaveExchangeRate(ExchangeRate exchangeRate);
    }
}