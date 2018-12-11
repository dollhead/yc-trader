using System.Collections.Generic;
using System.Linq;

namespace YCTrader.Services.Predictor
{
    public class TimeSeries
    {
        private readonly long _endUnixEpochTime;
        private readonly long _startUnixEpochTime;
        private readonly decimal[] _values;

        public TimeSeries(IDictionary<long, decimal> values, int period)
        {
            var orderedDates = values.Keys.ToList();
            orderedDates.Sort();

            _startUnixEpochTime = orderedDates.First();
            _endUnixEpochTime = orderedDates.Last();

            var seriesLength = (_endUnixEpochTime - _startUnixEpochTime) / period;
            _values = new decimal[seriesLength];
            _values[0] = values[_startUnixEpochTime];
            
            for (long i = 1; i < seriesLength; i++)
            {
                var index = orderedDates.BinarySearch(_startUnixEpochTime + i);
                if (index < 0) index = ~index;

                _values[i] = values[orderedDates[index]];
            }
        }

        public decimal GetMovingAverage()
        {
            return _values.Average();
        }
    }
}