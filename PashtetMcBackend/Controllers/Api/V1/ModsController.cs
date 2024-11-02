using System.Net;
using BronuhMcBackend.Models.Api;
using Common.Api;
using Common.IO.Checksum;
using Microsoft.AspNetCore.Mvc;

namespace BronuhMcBackend.Controllers.Api.V1;

[Route("api/v1/mods")]
[ApiController]
public class ModsController : ControllerBase
{
    private ILogger _logger;
    private readonly IApiProvider _apiProvider;
    private readonly IChecksumProvider _checksumProvider;

    public ModsController(ILogger<ModsController> logger, IApiProvider apiProvider, IChecksumProvider checksumProvider)
    {
        _logger = logger;
        _apiProvider = apiProvider;
        _checksumProvider = checksumProvider;
    }
    
    // GET: api/v1/mods/required/list
    [HttpGet("required/list")]
    public IActionResult GetRequiredModsList()
    {
        _logger.LogInformation("Mods list requested from {user}", Request.HttpContext.Connection.RemoteIpAddress?.ToString());
        var modsLocal = _apiProvider.GetRequiredModsList();
        var baseUrl = Url.Action(nameof(DownloadRequiredMod), new { modName = "" });
        _logger.LogInformation("Base URL is {baseUrl}", baseUrl);
        
        var modsRemote = new RemoteFilesList(modsLocal, baseUrl, _checksumProvider);

        return Ok(modsRemote);
    }
    
    // GET: api/v1/mods/optional/list
    [HttpGet("optional/list")]
    public IActionResult GetOptionalModsList()
    {
        var modsLocal = _apiProvider.GetOptionalModsList();
        var baseUrl = Url.Action(nameof(DownloadRequiredMod), new { modName = "" });
        
        var modsRemote = new RemoteFilesList(modsLocal, baseUrl, _checksumProvider);

        return Ok(modsRemote);
    }
    
    // GET: api/v1/mods/required/download/{*modName}
    [HttpGet("required/download/{*modName}")]
    public IActionResult DownloadRequiredMod(string modName)
    {
        var modFile = _apiProvider.GetRequiredModFile(modName);

        if (modFile.Exists())
        {
            return File(modFile.GetBytes(), "application/octet-stream", modFile.FileName);
        }

        return NotFound("File not found");
    }
    
    // GET: api/v1/mods/optional/download/{*modName}
    [HttpGet("optional/download/{*modName}")]
    public IActionResult DownloadOptionalMod(string modName)
    {
        var modFile = _apiProvider.GetOptionalModFile(modName);

        if (modFile.Exists())
        {
            return File(modFile.GetBytes(), "application/octet-stream", modFile.FileName);
        }

        return NotFound("File not found");
    }
}