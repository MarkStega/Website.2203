using Microsoft.AspNetCore.Http;

namespace Website.Lib;


/// <summary>
/// Adds response headers to prevent caching.
/// </summary>
public class NoCacheMiddleware
{
    private readonly RequestDelegate _next;
    
    
    public NoCacheMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Response.OnStarting(() =>
        {
            // ref: <a href="http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers">http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers</a>
            httpContext.Response.Headers.Append("Cache-Control", "public, max-age=86400");
            httpContext.Response.Headers.Append("Pragma", "no-cache");
            httpContext.Response.Headers.Append("Expires", "0");
            return Task.FromResult(0);
        });

        await _next.Invoke(httpContext);
    }
}