using CargoX.Fridge.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CargoX.Fridge.Web.Host.Controllers
{
    [Produces("application/json")]
    [Route("api/Health")]
    public class HealthController : FridgeControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("ok");
    }
}