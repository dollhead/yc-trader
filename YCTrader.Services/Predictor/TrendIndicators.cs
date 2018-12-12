using System.Linq;

namespace YCTrader.Services.Predictor
{
    public static class TrendIndicators
    {
        public static decimal GetExponentialMovingAverage(decimal[] data)
        {
            var numberOfPreviousValuesToConsider = data.Length / 2;
            if (numberOfPreviousValuesToConsider == 0)
            {
                throw new NotEnoughDataException("There is not enough data to compute moving average");
            }
            
            var valuesToConsider = data.TakeLast(numberOfPreviousValuesToConsider).ToList();
            var multiplier = 2 / (numberOfPreviousValuesToConsider + 1);

            var currentExponentialMovingAverage = data
                .SkipLast(numberOfPreviousValuesToConsider)
                .TakeLast(numberOfPreviousValuesToConsider)
                .Average();
            
            for (var i = 0; i < numberOfPreviousValuesToConsider; i++)
            {
                currentExponentialMovingAverage = multiplier * valuesToConsider[i] + (1 - multiplier) * currentExponentialMovingAverage;
            }

            return currentExponentialMovingAverage;
        }
    }
}