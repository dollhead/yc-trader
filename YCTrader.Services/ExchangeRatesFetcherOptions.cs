namespace YCTrader.Services
{
    public class ExchangeRatesFetcherOptions
    {
        public string ExchangeRatesServiceApiUrl { get; set; }
        
        public int FetchInterval { get; set; }
    }
}