using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using YCTrader.Services;
using YCTrader.Services.Predictor;

namespace YCTrader.Console
{
    public class ExchangeRateStatus
    {
        public decimal LatestPrice { get; set; }

        public DateTime TimeStamp { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ExchangeRateTrend Trend { get; set; }

        public static ExchangeRateStatus ToExchangeRateStatus(ExchangeRate rate, ExchangeRateTrend trend)
        {
            var status = new ExchangeRateStatus
            {
                Trend = trend,
                LatestPrice = rate.Price,
                TimeStamp = DateTimeOffset.FromUnixTimeSeconds(rate.Timestamp).LocalDateTime
            };

            return status;
        }
    }
}