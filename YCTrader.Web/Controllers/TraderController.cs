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
        public ActionResult<ExchangeRateResponse> GetPrediction()
        {
            var response = _exchangeRatePredictor.GetPrediction();
            response.Rate.Price = _exchangeRatesProvider.GetLatestExchangeRate();
            return new OkObjectResult(response);
        }
    }
}