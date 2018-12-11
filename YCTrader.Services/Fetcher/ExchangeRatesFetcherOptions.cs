namespace YCTrader.Services.Fetcher
{
    public class ExchangeRatesFetcherOptions
    {
        public ExchangeRatesFetcherOptions()
        {
            MaxExchangeRateMultiplier = 1.3;
            MinExchangeRateMultiplier = 1.1;
        }

        public string ExchangeRatesServiceApiUrl { get; set; }

        public int FetchInterval { get; set; }
        
        public int NumberOfRetries { get; set; }
        
        public int NumberOfPreviousDaysToFetch { get; set; }
        
        public double MaxExchangeRateMultiplier { get; set; }
        
        public double MinExchangeRateMultiplier { get; set; }
    }
}