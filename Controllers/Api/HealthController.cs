using EnterBridgePOC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterBridgePOC.Controllers.Api
{
    public class HealthController(IEnterBridgeApiService api) : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await api.GetHealthAsync());
    }
}
