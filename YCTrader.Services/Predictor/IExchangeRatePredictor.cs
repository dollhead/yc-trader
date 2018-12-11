namespace YCTrader.Services.Predictor
{
    public interface IExchangeRatePredictor
    {
        ExchangeRateResponse GetPrediction();
    }
}