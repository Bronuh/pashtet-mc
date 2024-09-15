using BronuhMcBackend.Models;
using BronuhMcBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BronuhMcBackend.Controllers;

[Route("api/core")]
[ApiController]
public class CoreController : ControllerBase
{
    private readonly FilesystemContext _filesCtx;
    private ILogger _logger;

    public CoreController(IOptions<DirectorySettings> options, FilesystemContext filesCtx, ILogger logger)
    {
        _filesCtx = filesCtx;
        _logger = logger;
    }
    
    // GET: api/core/jre
    [HttpGet("jre")]
    public IActionResult DownloadJre(string modName)
    {
        var file = _filesCtx.GetFile(_filesCtx.AsAbsolute(_filesCtx.JreFilePath));
        var fullPath = file.AbsolutePath;

        if (System.IO.File.Exists(fullPath))
        {
            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "application/octet-stream", Path.GetFileName(fullPath));
        }

        return NotFound("File not found");
    }
    
    // GET: api/core/minecraft
    [HttpGet("minecraft")]
    public IActionResult DownloadMinecraft(string modName)
    {
        var file = _filesCtx.GetFile(_filesCtx.AsAbsolute(_filesCtx.MinecraftFilePath));
        var fullPath = file.AbsolutePath;

        if (System.IO.File.Exists(fullPath))
        {
            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "application/octet-stream", Path.GetFileName(fullPath));
        }

        return NotFound("File not found");
    }
}