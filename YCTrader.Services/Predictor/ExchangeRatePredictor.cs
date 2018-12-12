using System;
using System.Linq;
using Microsoft.Extensions.Options;
using YCTrader.Services.Storage;

namespace YCTrader.Services.Predictor
{
    public class ExchangeRatePredictor : IExchangeRatePredictor
    {
        private readonly IExchangeRatesStorage _exchangeRatesStorage;
        private readonly IOptions<ExchangeRatePredictorOptions> _options;

        public ExchangeRatePredictor(IExchangeRatesStorage exchangeRatesStorage,
            IOptions<ExchangeRatePredictorOptions> options)
        {
            _exchangeRatesStorage = exchangeRatesStorage;
            _options = options;
        }

        public ExchangeRateTrend GetExchangeRateTrend()
        {
            var exchangeRatesForCurrentDay = _exchangeRatesStorage.GetExchangeRatesForCurrentDay();
            if (!exchangeRatesForCurrentDay.Any())
            {
                return ExchangeRateTrend.Unknown;
            }

            try
            {
                var currentDayTimeSeries = new TimeSeries(exchangeRatesForCurrentDay, _options.Value.TimeSeriesPeriod);
                var currentDayMovingAverage = TrendIndicators.GetExponentialMovingAverage(currentDayTimeSeries.Values);

                var previousDaysToConsider = _options.Value.NumberOfPreviousDaysToConsider;
                var previousDaysMovingAverage = TrendIndicators.GetExponentialMovingAverage(_exchangeRatesStorage
                    .GetClosingPricesForPreviousDays(previousDaysToConsider).ToArray());

                return GetExchangeRateTrend(currentDayMovingAverage, previousDaysMovingAverage);
            }
            catch (NotEnoughDataException)
            {
                return ExchangeRateTrend.Unknown;
            }
        }

        private ExchangeRateTrend GetExchangeRateTrend(decimal currentDayMovingAverage, decimal previousDaysMovingAverage)
        {
            var movingAveragesRatio = (float)((currentDayMovingAverage - previousDaysMovingAverage) / previousDaysMovingAverage);

            var quantiles = _options.Value.TrendQuantiles;
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