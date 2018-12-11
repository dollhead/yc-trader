using System;
using Microsoft.Extensions.Options;
using YCTrader.Services.Storage;

namespace YCTrader.Services.Predictor
{
    public class ExchangeRatePredictor : IExchangeRatePredictor
    {
        private readonly IExchangeRatesStorage _exchangeRatesStorage;
        private readonly IOptionsMonitor<ExchangeRatePredictorOptions> _options;

        public ExchangeRatePredictor(IExchangeRatesStorage exchangeRatesStorage,
            IOptionsMonitor<ExchangeRatePredictorOptions> options)
        {
            _exchangeRatesStorage = exchangeRatesStorage;
            _options = options;
        }

        public ExchangeRateResponse GetPrediction()
        {
            var latestExchangeRate = _exchangeRatesStorage.GetLatestExchangeRate();

            var exchangeRatesForCurrentDay = _exchangeRatesStorage.GetExchangeRatesForCurrentDay();
            var currentDayTimeSeries =
                new TimeSeries(exchangeRatesForCurrentDay, _options.CurrentValue.TimeSeriesPeriod);
            var currentDayMovingAverage = currentDayTimeSeries.GetMovingAverage();

            var exchangeRatesForPreviousDays = _exchangeRatesStorage
                .GetExchangeRatesForPreviousDays(_options.CurrentValue.NumberOfPreviousDaysToConsider);
            var previousDaysTimeSeries =
                new TimeSeries(exchangeRatesForPreviousDays, _options.CurrentValue.TimeSeriesPeriod);
            var previousDaysMovingAverage = previousDaysTimeSeries.GetMovingAverage();

            var movingAveragesRatio = (currentDayMovingAverage - previousDaysMovingAverage) / currentDayMovingAverage;

            return new ExchangeRateResponse
            {
                Rate = new ExchangeRate
                {
                    Price = latestExchangeRate,
                    Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
                },
                Trend = movingAveragesRatio > 0 ? ExchangeRateTrend.DecreasingFast : ExchangeRateTrend.IncreasingFast
            };
        }
    }
}