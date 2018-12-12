using System;
using System.Collections.Generic;
using System.Linq;

namespace YCTrader.Services.Predictor
{
    public class TimeSeries
    {
        private readonly long _endUnixEpochTime;
        private readonly long _startUnixEpochTime;
        
        public TimeSeries(IDictionary<long, decimal> values, int period)
        {           
            var orderedDates = values.Keys.ToList();
            orderedDates.Sort();

            _startUnixEpochTime = orderedDates.First();
            _endUnixEpochTime = orderedDates.Last();

            ResampleAndFillMissingValues(values, orderedDates, period);
        }
        
        public decimal[] Values { get; private set; }
        
        private void ResampleAndFillMissingValues(IDictionary<long, decimal> values,
            List<long> orderedDates,
            int newPeriod)
        {
            var seriesLength = ((_endUnixEpochTime - _startUnixEpochTime) / newPeriod) + 1;
            Values = new decimal[seriesLength];
            Values[0] = values[_startUnixEpochTime];

            for (long i = 1; i < seriesLength; i++)
            {
                var index = orderedDates.BinarySearch(_startUnixEpochTime + i);
                if (index < 0) index = ~index;

                Values[i] = values[orderedDates[index]];
            }
        }
    }
}