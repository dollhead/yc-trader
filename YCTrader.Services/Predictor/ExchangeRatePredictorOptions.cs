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
        public float DecreasingFast { get; set; }

        public float Decreasing { get; set; }

        public float IncreasingFast { get; set; }

        public float Increasing { get; set; }
    }
}