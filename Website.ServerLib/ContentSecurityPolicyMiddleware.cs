using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Website.Lib;


/// <summary>
/// Middleware that adds the content security policy to HTTP request headers. Also pump primes the Vectis server at startup to
/// ensure that the server populates its caches as early as possible.
/// </summary>
public class ContentSecurityPolicyMiddleware
{
    private readonly RequestDelegate _next;


    public ContentSecurityPolicyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cspService = context.RequestServices.GetService<ContentSecurityPolicyService>();
        var webHostEnvironment = context.RequestServices.GetService<IWebHostEnvironment>();

        var source = (webHostEnvironment.IsDevelopment() ? "'self' " : "") + $"'nonce-{cspService.NonceValue}'";

        var baseUri = context.Request.Host.ToString();
        var baseDomain = context.Request.Host.Host;

        var csp =
            "base-uri 'self'; " +
            "block-all-mixed-content; " +
            "child-src 'self' ; " +
            $"connect-src 'self' wss://{baseDomain}:* www.google-analytics.com region1.google-analytics.com; " +
            "default-src 'self'; " +
            "font-src use.typekit.net fonts.googleapis.com fonts.gstatic.com; " +
            "frame-ancestors 'none'; " +
            "frame-src 'self'; " +
            "form-action 'none'; " +
            "img-src 'self' www.google-analytics.com *.openstreetmap.org data: w3.org/svg/2000; " +
            "manifest-src 'self'; " +
            "media-src 'self'; " +
            "prefetch-src 'self'; " +
            "object-src  data: 'unsafe-eval'; " +
            $"report-to https://{baseUri}/api/CspReporting/UriReport; " +
            $"report-uri https://{baseUri}/api/CspReporting/UriReport; " +
            // The sha-256 hash relates to an inline script added by blazor's javascript
            $"script-src {cspService.ScriptSrc} 'sha256-v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA=' 'report-sample' 'unsafe-eval' localhost:* https://www.googletagmanager.com; " +
            "style-src 'self' 'unsafe-inline' 'report-sample' p.typekit.net use.typekit.net fonts.googleapis.com fonts.gstatic.com; " +
            "upgrade-insecure-requests; " +
            "worker-src 'self';";

        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
        context.Response.Headers.Add("X-ClientId", "dioptra");
        context.Response.Headers.Add("Referrer-Policy", "no-referrer");
        context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
        context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        context.Response.Headers.Add("Content-Security-Policy", csp);

        await _next(context);
    }
}


public static partial class MiddlewareExtensions
{
    /// <summary>
    /// Middleware that pump primes the Vectis server, ensuring that it populates caches from the database. Place at the start of the middleware sequence.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ContentSecurityPolicyMiddleware>();
    }
}