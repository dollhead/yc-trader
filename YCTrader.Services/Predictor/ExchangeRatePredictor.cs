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

        public ExchangeRateTrend GetExchangeRateTrend()
        {
            var exchangeRatesForCurrentDay = _exchangeRatesStorage.GetExchangeRatesForCurrentDay();
            var currentDayTimeSeries =
                new TimeSeries(exchangeRatesForCurrentDay, _options.CurrentValue.TimeSeriesPeriod);
            var currentDayMovingAverage = currentDayTimeSeries.GetMovingAverage();

            var exchangeRatesForPreviousDays = _exchangeRatesStorage
                .GetExchangeRatesForPreviousDays(_options.CurrentValue.NumberOfPreviousDaysToConsider);
            var previousDaysTimeSeries =
                new TimeSeries(exchangeRatesForPreviousDays, _options.CurrentValue.TimeSeriesPeriod);
            var previousDaysMovingAverage = previousDaysTimeSeries.GetMovingAverage();

            return GetExchangeRateTrend(currentDayMovingAverage, previousDaysMovingAverage);
        }

        private ExchangeRateTrend GetExchangeRateTrend(decimal currentDayMovingAverage, decimal previousDaysMovingAverage)
        {
            var movingAveragesRatio = (float)((currentDayMovingAverage - previousDaysMovingAverage) / previousDaysMovingAverage);

            var quantiles = _options.CurrentValue.TrendQuantiles;
            if (movingAveragesRatio < quantiles.DecreasingFast)
            {
                return ExchangeRateTrend.DecreasingFast;
            }

            if (movingAveragesRatio < quantiles.Decreasing)
            {
                return ExchangeRateTrend.Decreasing;
            }

            if (movingAveragesRatio < quantiles.Increasing)
            {
                return ExchangeRateTrend.Stable;
            }

            if (movingAveragesRatio < quantiles.IncreasingFast)
            {
                return ExchangeRateTrend.Increasing;
            }

            return ExchangeRateTrend.IncreasingFast;
        }
    }
}