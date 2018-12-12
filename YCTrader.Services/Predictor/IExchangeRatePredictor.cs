namespace YCTrader.Services.Predictor
{
    public interface IExchangeRatePredictor
    {
        ExchangeRateTrend GetExchangeRateTrend();
    }
}