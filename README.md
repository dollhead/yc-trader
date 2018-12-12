## Test problem for YC

:warning: uses https://dotnet.microsoft.com/download/dotnet-core/2.2 :warning:
This application fetches currency exchange rate with a configured time interval from https://forex.1forge.com/. When starting, it also generates closing prices of exchange rate for specified number of previous days, simulating historical data. Closing prices are generated using current rate and `MaxExchangeRateMultiplier`/`MinExchangeMultiplier` values from `appsettings.json`:

```
closingPrice = random value between currentRate * MinExchangeRateMultiplier and currentRate * MaxExchangeRateMultiplier
```

Application listens to http requests on http://localhost:5000/prediction. When GET request is received, it calculates exponential moving average(EMA) for the current date and compares it to EMA of previous days. EMA is always calculated using last `timeseries.Length/2` values. Before calculating EMA for current day, data for the current day is resampled to time period, specified with `timeSeriesPeriod` value from `appsetings.json`. When resampling, missing values are filled with the closest larger value using the behavior of [`List<T>.BinarySearch`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.binarysearch?view=netframework-4.7.2) method:

>The zero-based index of item in the sorted List<T>, if item is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than item or, if there is no larger element, the bitwise complement of Count.

After EMA calcualtions, application returns current exchange rate with local timestamp and exchange rate trend. There are 6 possible trend values:

```
    public enum ExchangeRateTrend
    {
        Unknown,

        DecreasingFast,

        Decreasing,

        Stable,

        Increasing,

        IncreasingFast
    }
```

`Unknown` value is returned when there is not enough data to calculate trend. Other values are returned depending on a ratio between current EMA and EMA for previous days, using trend quantiles from `appsettings.json`:

```
    "trendQuantiles": {
      "decreasingFast": -0.3,
      "decreasing": -0.1,
      "increasing": 0.1,
      "increasingFast": 0.3
    }
```

Requests to https://forex.1forge.com/ are sended using `ExchangeRatesServiceClient` which has an HttpClient, injected by HttpClientFactory. If a transient error occurs while communicating with exchange rates service, requests will be retried for the number of times, specified in `exchangeRatesFetcherOptions` from `appsettings.json`(HttpClientFactory Polly integration is used for this).

As a result, a following JSON object is returned:

```
curl http://localhost:5000/prediction

{"LatestPrice":1.1334,"TimeStamp":"2018-12-12T14:06:49+02:00","Trend":"Decreasing"}
```

## Known issues

- Only one trend indicator is used
- No validation for options
- Exchange service url in appsetings.json contains access token which is not secure, should be stored somewhere else(e.g., env variable)
- No unit tests
- Algorithm for filling missing values is primitive, probably some kind of interpolation should be used
