using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Website.Server;

/// <summary>
/// And endpoint for CSP reporting.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CspReportingController : Controller
{
    /// <summary>
    /// Receives CSP reports.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("UriReport")]
    [AllowAnonymous]
    public async Task<IActionResult> UriReport([FromForm] string request)
    {
        await Task.CompletedTask;
        Log.Warning("CSP violation: " + request);
        return Ok();
    }
}
