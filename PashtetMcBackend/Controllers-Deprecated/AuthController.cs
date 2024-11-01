using BhCommon;
using BronuhMcBackend.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BronuhMcBackend.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private const string LoginHeader = "Login";
    private const string PasswordHeader = "Password";
    private readonly FilesystemContext _filesCtx;
    private readonly PasswordHasher _passwordHasher;

    public AuthController(FilesystemContext filesCtx, PasswordHasher passwordHasher)
    {
        _filesCtx = filesCtx;
        _passwordHasher = passwordHasher;
        SetupAuth();
    }

    // GET: api/auth
    [HttpGet]
    public IActionResult Auth()
    {
        var headers = Request.Headers;
        var (isValid, msg, login, password) = ValidateHeaders(headers);

        if (!isValid)
        {
            return BadRequest(msg);
        }

        var userAuthPath = _filesCtx.AsAbsolute(Path.Combine(_filesCtx.AuthDirPath, login));
        if (!System.IO.File.Exists(userAuthPath))
        {
            return BadRequest("Not registered");
        }
        var storedPasswordHash = System.IO.File.ReadAllText(userAuthPath);
        var isPassValid = _passwordHasher.VerifyPassword(password, storedPasswordHash);

        if (isPassValid)
        {
            return Ok("true");
        }
        else
        {
            return Unauthorized("false");
        }
    }
    
    
    // GET: api/auth/register
    [HttpGet("register")]
    public IActionResult Register()
    {
        var headers = Request.Headers;
        var (isValid, msg, login, password) = ValidateHeaders(headers);

        if (!isValid)
        {
            return BadRequest(msg);
        }
        
        var userAuthPath = _filesCtx.AsAbsolute(Path.Combine(_filesCtx.AuthDirPath, login));
        if (System.IO.File.Exists(userAuthPath))
        {
            return BadRequest("Already registered");
        }
        
        var hash = _passwordHasher.HashPassword(password);
        System.IO.File.WriteAllText(userAuthPath, hash);
        
        return Ok("ok");
    }

    private (bool, string, string, string) ValidateHeaders(IHeaderDictionary headers)
    {
        if (!headers.TryGetValue(LoginHeader, out var login))
        {
            return (false, "no-login", "", "");
        }

        if (!headers.TryGetValue(PasswordHeader, out var password))
        {
            return (false, "no-password", "", "");
        }

        if (String.IsNullOrWhiteSpace(login))
        {
            return (false, "empty-login", "", "");
        }

        if (String.IsNullOrWhiteSpace(password))
        {
            return (false, "empty-password", "", "");
        }
        
        return (true, "ok", login, password)!;
    }

    private void SetupAuth()
    {
        Directory.CreateDirectory(_filesCtx.AsAbsolute(_filesCtx.AuthDirPath));
    }
}