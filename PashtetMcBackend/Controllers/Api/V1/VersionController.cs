#region

using Common.IO.Checksum;
using Microsoft.AspNetCore.Mvc;
using PashtetMcBackend.Models.Api;

#endregion

namespace PashtetMcBackend.Controllers.Api.V1;

[Route("api/v1/version")]
[ApiController]
public class VersionController : ControllerBase
{
    private ILogger _logger;
    private readonly IApiProvider _apiProvider;
    private readonly IChecksumProvider _checksumProvider;

    public VersionController(ILogger<CoreController> logger, IApiProvider apiProvider, IChecksumProvider checksumProvider)
    {
        _logger = logger;
        _apiProvider = apiProvider;
        _checksumProvider = checksumProvider;
    }
    
    // GET: api/v1/version
    public IActionResult DownloadVersion()
    {
        var file = _apiProvider.GetVersionFile();

        if (file.Exists())
        {
            var fileBytes = file.GetBytes();
            return File(fileBytes, "application/octet-stream", file.FileName);
        }

        return NotFound("File not found");
    }
    
    // GET: api/v1/version/info
    [HttpGet("info")]
    public IActionResult GetVersionInfo()
    {
        var file = _apiProvider.GetVersionFile();
        var downloadUrl = $"{Request.Scheme}://{Request.Host}{Url.Action(nameof(DownloadVersion))}";
        
        if (file.Exists())
        {
            var remoteFile = file.AsRemote(downloadUrl, _checksumProvider);
            return Ok(remoteFile);
        }

        return NotFound("File not found");
    }
}