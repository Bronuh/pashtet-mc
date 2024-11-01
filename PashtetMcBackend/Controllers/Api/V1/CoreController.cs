﻿using System.Net;
using System.Net.Mime;
using BronuhMcBackend.Models.Api;
using Common.Api;
using Microsoft.AspNetCore.Mvc;

namespace BronuhMcBackend.Controllers.Api.V1;

[Route("api/v1/core")]
[ApiController]
public class CoreController : ControllerBase
{
    private ILogger _logger;
    private readonly IApiProvider _apiProvider;

    public CoreController(ILogger<CoreController> logger, IApiProvider apiProvider)
    {
        _logger = logger;
        _apiProvider = apiProvider;
    }
    
    // GET: api/v1/core/jre
    [HttpGet("jre")]
    public IActionResult DownloadJre()
    {
        var file = _apiProvider.GetJavaFile();
        var fullPath = file.AbsolutePath;

        if (file.Exists())
        {
            var fileBytes = file.GetBytes();
            return File(fileBytes, "application/octet-stream", file.FileName);
        }

        return NotFound("File not found");
    }
    
    // GET: api/v1/core/minecraft
    [HttpGet("minecraft")]
    public IActionResult DownloadMinecraft()
    {
        var file = _apiProvider.GetMinecraftFile();
        var fullPath = file.AbsolutePath;

        if (file.Exists())
        {
            var fileBytes = file.GetBytes();
            return File(fileBytes, "application/octet-stream", file.FileName);
        }

        return NotFound("File not found");
    }
}