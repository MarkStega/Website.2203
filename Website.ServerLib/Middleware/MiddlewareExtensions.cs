using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace Website.Lib;


/// <summary>
/// Middleware extensions
/// </summary>
public static class MiddlewareExtensions
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


    /// <summary>
    /// Middleware that pump primes the Vectis server, ensuring that it populates caches from the database. Place at the start of the middleware sequence.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseNoCacheMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<NoCacheMiddleware>();
    }


#if FUTURE_EXPERIEMENT // This is where to experiment with delivering pre-compressed static assets
    public static StaticFileOptions StaticFileOptions => new() { OnPrepareResponse = OnPrepareResponse };


    private static void OnPrepareResponse(StaticFileResponseContext context)
    {
        var file = context.File;
        var request = context.Context.Request;
        var response = context.Context.Response;

        if (file.Name.EndsWith(".gz"))
        {
            response.Headers[HeaderNames.ContentEncoding] = "gzip";
            return;
        }

        if (file.Name.IndexOf(".min.", StringComparison.OrdinalIgnoreCase) != -1)
        {
            var requestPath = request.Path.Value;
            var filePath = file.PhysicalPath;

            //if (IsDevelopment)
            //{
            //    if (File.Exists(filePath.Replace(".min.", ".")))
            //    {
            //        response.StatusCode = (int)HttpStatusCode.TemporaryRedirect;
            //        response.Headers[HeaderNames.Location] = requestPath.Replace(".min.", ".");
            //    }
            //}
            //else
            //{
                var acceptEncoding = (string)request.Headers[HeaderNames.AcceptEncoding];
                if (acceptEncoding.IndexOf("gzip", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    if (File.Exists(filePath + ".gz"))
                    {
                        response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                        response.Headers[HeaderNames.Location] = requestPath + ".gz";
                    }
                }
            //}
        }
    }
#endif
}
