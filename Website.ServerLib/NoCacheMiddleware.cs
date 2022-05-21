using Microsoft.AspNetCore.Http;

namespace Website.Lib;
public class NoCacheMiddleware
{
    private readonly RequestDelegate m_next;
    public NoCacheMiddleware(RequestDelegate next)
    {
        m_next = next;
    }
    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Response.OnStarting((state) =>
        {
            // ref: <a href="http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers">http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers</a>
            httpContext.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
            httpContext.Response.Headers.Append("Pragma", "no-cache");
            httpContext.Response.Headers.Append("Expires", "0");
            return Task.FromResult(0);
        }, null);
        await m_next.Invoke(httpContext);
    }
}