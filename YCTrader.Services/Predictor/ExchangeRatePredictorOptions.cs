namespace YCTrader.Services.Predictor
{
    public class ExchangeRatePredictorOptions
    {
        public int NumberOfPreviousDaysToConsider { get; set; }

        public TrendQuantiles TrendQuantiles { get; set; }
        
        public int TimeSeriesPeriod { get; set; }
    }

    public class TrendQuantiles
    {
        public int DecreasingFast { get; set; }

        public int Decreasing { get; set; }

        public int IncreasingFast { get; set; }

        public int Increasing { get; set; }
    }
}