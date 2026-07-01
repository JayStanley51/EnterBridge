using Microsoft.AspNetCore.Mvc;

namespace EnterBridgePOC.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class ApiControllerBase : ControllerBase
    {
    }
}
