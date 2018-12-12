using Microsoft.AspNetCore.Mvc;
using YCTrader.Services.Predictor;
using YCTrader.Services.Storage;

namespace YCTrader.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraderController : ControllerBase
    {
        private readonly IExchangeRatesStorage _exchangeRatesProvider;
        private readonly IExchangeRatePredictor _exchangeRatePredictor;

        public TraderController(IExchangeRatesStorage exchangeRatesProvider, IExchangeRatePredictor exchangeRatePredictor)
        {
            _exchangeRatePredictor = exchangeRatePredictor;
            _exchangeRatesProvider = exchangeRatesProvider;
        }

        [HttpGet]
        public ActionResult GetPrediction()
        {
            var exchangeRateTrend = _exchangeRatePredictor.GetExchangeRateTrend();
            var currentExchangeRate = _exchangeRatesProvider.GetLatestExchangeRate();
            return new OkObjectResult(new {currentExchangeRate, exchangeRateTrend});
        }
    }
}