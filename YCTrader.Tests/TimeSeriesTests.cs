using System;
using System.Collections.Generic;
using Xunit;
using YCTrader.Services.Predictor;

namespace YCTrader.Tests
{
    public class TimeSeriesTests
    {
        [Fact]
        public void Test1()
        {
            var random = new Random();

            var period = 3;
            var dict = new Dictionary<long, decimal>();

            for (var i = 0; i < 100; i++)
            {
                var rate = (random.NextDouble() + 1) * 1.5;
                var timestamp = random.Next(i, i + period);
                dict[timestamp] = (decimal)rate;
            }

            var timeseries = new TimeSeries(dict, period);
            
            Assert.True(timeseries.GetMovingAverage() != 0);
        }
    }
}