using Microsoft.AspNetCore.Mvc;

namespace CarPartBotApi.Api.Contollers;

[ApiController]
[Route("api/[controller]")]
public class PingController(IHostEnvironment _hostEnvironment) : ControllerBase
{
    /// <summary>
    /// A simple endpoint to verify the API is up and responding to HTTP requests.
    /// </summary>
    [HttpGet]
    public IActionResult Ping() // TODO Protect
    {
        return Ok(new
        {
            message = "pong",
            timestamp = DateTimeOffset.UtcNow,
            environment = _hostEnvironment.EnvironmentName
        });
    }
}
