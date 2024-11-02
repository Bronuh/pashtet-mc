using BronuhMcBackend.Models.Api;
using Microsoft.AspNetCore.Mvc;

namespace BronuhMcBackend.Controllers.Api.V1;

[Route("api/v1/data")]
[ApiController]
public class DataController : ControllerBase
{
    private ILogger _logger;
    private readonly IApiProvider _apiProvider;

    public DataController(ILogger<CoreController> logger, IApiProvider apiProvider)
    {
        _logger = logger;
        _apiProvider = apiProvider;
    }
    
    // GET: api/v1/data/servers
    [HttpGet("servers")]
    public IActionResult DownloadJre()
    {
        var file = _apiProvider.GetServersFile();

        if (file.Exists())
        {
            var fileBytes = file.GetBytes();
            return File(fileBytes, "application/octet-stream", file.FileName);
        }

        return NotFound("File not found");
    }
}