using Microsoft.AspNetCore.Mvc;

namespace BronuhMcBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }
    [Route("notfound")]
    public IActionResult NotFoundHandler()
    {
        _logger.LogWarning("Attempt to access non-existing route {route}", Request.Path.Value);
        return NotFound(new { message = "This route does not exist." });
    }
}