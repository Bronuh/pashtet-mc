using BronuhMcBackend.Models;
using BronuhMcBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BronuhMcBackend.Controllers;

[Route("api/mods")]
[ApiController]
public class ModsController : ControllerBase
{
    private readonly string _rootDirectory;
    private readonly string _snapshotsDirectory;
    private readonly string _currentDirectory;
    private readonly string _modsDirectory;
    private readonly FilesystemContext _filesCtx;

    public ModsController(IOptions<DirectorySettings> options, FilesystemContext filesCtx)
    {
        _rootDirectory = options.Value.RootDirectory;
        _snapshotsDirectory = Path.Combine(_rootDirectory, "snapshots");
        _currentDirectory = Path.Combine(_snapshotsDirectory, "current");
        _modsDirectory = Path.Combine(_rootDirectory, "mods");
        _filesCtx = filesCtx;
    }
    
    // GET: api/mods/list
    [HttpGet("list")]
    public IActionResult GetModsList()
    {
        var mods = _filesCtx.GetFiles(_filesCtx.ModsDirPath, loose: false);
        return Ok(mods);
    }
    
    // GET: api/mods/download/{*modname}
    [HttpGet("download/{*modName}")]
    public IActionResult DownloadFile(string modName)
    {
        var file = _filesCtx.GetFile(Path.Combine(_filesCtx.RootPath, _filesCtx.ModsDirPath, modName));
        var fullPath = file.AbsolutePath;

        if (System.IO.File.Exists(fullPath))
        {
            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "application/octet-stream", Path.GetFileName(fullPath));
        }

        return NotFound("File not found");
    }
}