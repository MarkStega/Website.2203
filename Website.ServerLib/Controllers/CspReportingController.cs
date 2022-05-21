using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Website.Lib.Controllers;

/// <summary>
/// And endpoint for CSP reporting.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CspReportingController : Controller
{
    [HttpPost("UriReport")]
    [AllowAnonymous]
    public async Task<IActionResult> UriReport([FromForm] string request)
    {
        await Task.CompletedTask;
        Log.Warning("CSP violation: " + request);
        return Ok();
    }
}
