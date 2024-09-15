using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using System.IO.Hashing;
using BronuhMcBackend.Models;
using BronuhMcBackend.Utils;

namespace BronuhMcBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DirectoryController : ControllerBase
{
    private readonly string _rootDirectory;
    private readonly FilesystemContext _filesCtx;
    private ILogger _logger;

    public DirectoryController(IOptions<DirectorySettings> options, FilesystemContext filesCtx, ILogger logger)
    {
        _rootDirectory = options.Value.RootDirectory;
        _filesCtx = filesCtx;
        _logger = logger;
    }

    // GET: api/directory
    [HttpGet]
    public IActionResult GetRoot()
    {
        return Ok(ListFilesAndDirectories(_rootDirectory));
    }

    // GET: api/directory/{subPath}
    [HttpGet("{*subPath}")]
    public IActionResult GetSubDirectory(string subPath)
    {
        var fullPath = String.IsNullOrEmpty(subPath) ? _rootDirectory : Path.Combine(_rootDirectory, subPath);
        if (Directory.Exists(fullPath) || System.IO.File.Exists(fullPath))
        {
            return Ok(ListFilesAndDirectories(fullPath));
        }
        
        return NotFound("Directory or file not found");
    }

    
    // GET: api/directory/download/{*filePath}
    [HttpGet("download/{*filePath}")]
    public IActionResult DownloadFile(string filePath)
    {
        var fullPath = Path.Combine(_rootDirectory, filePath);

        if (System.IO.File.Exists(fullPath))
        {
            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, "application/octet-stream", Path.GetFileName(fullPath));
        }

        return NotFound("File not found");
    }
    
    // GET: api/directory/details/{*subPath}
    [HttpGet("details/{*subPath}")]
    public IActionResult DetailedFiles(string subPath = "")
    {
        var fullPath = String.IsNullOrEmpty(subPath) ? _rootDirectory : Path.Combine(_rootDirectory, subPath);

        if (Directory.Exists(fullPath))
        {
            return Ok(ListDetailedFiles(fullPath));
        }

        return NotFound("Directory not found");
    }
    
    // GET: api/directory/details
    [HttpGet("details")]
    public IActionResult DetailedFiles()
    {
        var fullPath = _rootDirectory;

        if (Directory.Exists(fullPath))
        {
            return Ok(ListDetailedFiles(fullPath));
        }

        return NotFound("Directory not found");
    }
    
    // GET: api/directory/tree/{*subPath}
    [HttpGet("tree/{*subPath}")]
    public IActionResult DetailedTree(string subPath)
    {
        var fullPath = String.IsNullOrEmpty(subPath) ? _rootDirectory : Path.Combine(_rootDirectory, subPath);

        if (Directory.Exists(fullPath))
        {
            return Ok(GetDirectoryTree(fullPath));
        }

        return NotFound("Directory not found");
    }
    
    // GET: api/directory/tree
    [HttpGet("tree")]
    public IActionResult DetailedTree()
    {
        var fullPath = _rootDirectory;

        if (Directory.Exists(fullPath))
        {
            return Ok(GetDirectoryTree(fullPath));
        }

        return NotFound("Directory not found");
    }
    
    

    private object ListFilesAndDirectories(string path)
    {
        var directories = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);

        return new
        {
            Directories = directories.Select(Path.GetFileName),
            Files = files.Select(Path.GetFileName)
        };
    }
    
    

    private object GetDirectoryTree(string dirPath)
    {
        return new
        {
            Name = Path.GetFileName(dirPath),
            Type = "directory",
            Directories = GetSubDirectories(dirPath),
            Files = GetFilesInDirectory(dirPath)
        };
    }

    private IEnumerable<object> GetSubDirectories(string dirPath)
    {
        var directories = Directory.GetDirectories(dirPath);
        foreach (var directory in directories)
        {
            yield return GetDirectoryTree(directory); // Recursively get subdirectory structure
        }
    }

    private IEnumerable<object> GetFilesInDirectory(string dirPath)
    {
        var files = Directory.GetFiles(dirPath);
        return files.Select(GetFileInfo);
    }

    private object ListDetailedFiles(string dirPath)
    {
        var files = Directory.GetFiles(dirPath);
        Url.Action("DownloadFile", new { filePath = dirPath });
        return new
        {
            Files = files.Select(GetFileInfo)
        };
    }

    private object GetFileInfo(string filePath)
    {
        // Get the file name
        var fileName = Path.GetFileName(filePath);

        // Calculate the file's checksum using xxHash64
        var checksum = CalculateFileChecksum(filePath);
        return new
        {
            Name = fileName,
            Checksum = checksum
        };
    }

    private string CalculateFileChecksum(string filePath)
    {
        using (var stream = System.IO.File.OpenRead(filePath))
        {
            var hash = new XxHash3();
            hash.Append(stream);
            var str = BitConverter.ToString(hash.GetCurrentHash()).Replace("-", "").ToLower();
            return str;
        }
    }
}