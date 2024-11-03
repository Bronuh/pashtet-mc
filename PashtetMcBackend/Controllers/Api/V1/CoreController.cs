#region

using Common.IO.Checksum;
using Microsoft.AspNetCore.Mvc;
using PashtetMcBackend.Models.Api;

#endregion

namespace PashtetMcBackend.Controllers.Api.V1;

[Route("api/v1/core")]
[ApiController]
public class CoreController : ControllerBase
{
    private ILogger _logger;
    private readonly IApiProvider _apiProvider;
    private readonly IChecksumProvider _checksumProvider;

    public CoreController(ILogger<CoreController> logger, IApiProvider apiProvider, IChecksumProvider checksumProvider)
    {
        _logger = logger;
        _apiProvider = apiProvider;
        _checksumProvider = checksumProvider;
    }
    
    // GET: api/v1/core/jre
    [HttpGet("jre")]
    public IActionResult DownloadJre()
    {
        var file = _apiProvider.GetJavaFile();

        if (file.Exists())
        {
            var fileBytes = file.GetBytes();
            return File(fileBytes, "application/octet-stream", file.FileName);
        }

        return NotFound("File not found");
    }
    
    // GET: api/v1/core/jre/info
    [HttpGet("jre/info")]
    public IActionResult GetJreInfo()
    {
        var file = _apiProvider.GetJavaFile();
        var downloadUrl = $"{Request.Scheme}://{Request.Host}{Url.Action(nameof(DownloadJre))}";
        
        if (file.Exists())
        {
            var remoteFile = file.AsRemote(downloadUrl, _checksumProvider);
            return Ok(remoteFile);
        }

        return NotFound("File not found");
    }
    
    // GET: api/v1/core/minecraft
    [HttpGet("minecraft")]
    public IActionResult DownloadMinecraft()
    {
        var file = _apiProvider.GetMinecraftFile();

        if (file.Exists())
        {
            var fileBytes = file.GetBytes();
            return File(fileBytes, "application/octet-stream", file.FileName);
        }

        return NotFound("File not found");
    }
    
    // GET: api/v1/core/minecraft/info
    [HttpGet("minecraft/info")]
    public IActionResult GetMinecraftInfo()
    {
        var file = _apiProvider.GetMinecraftFile();
        var downloadUrl = $"{Request.Scheme}://{Request.Host}{Url.Action(nameof(DownloadMinecraft))}";
        
        if (file.Exists())
        {
            var remoteFile = file.AsRemote(downloadUrl, _checksumProvider);
            return Ok(remoteFile);
        }

        return NotFound("File not found");
    }
}