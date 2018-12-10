using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using YCTrader.Services;

namespace YCTrader.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraderController : ControllerBase
    {
        private readonly IExchangeRatesStorage _exchangeRatesProvider;

        public TraderController(IExchangeRatesStorage exchangeRatesProvider)
        {
            _exchangeRatesProvider = exchangeRatesProvider;
        }
        
        [HttpGet]
        public ActionResult<decimal> Get()
        {
            return new OkObjectResult(_exchangeRatesProvider.GetLatestExchangeRate());
        }
    }
}