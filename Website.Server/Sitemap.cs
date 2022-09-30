using Microsoft.AspNetCore.Components;
using System.Reflection;
using Website.Lib;

namespace Website.Server;


/// <summary>
/// Generates a sitemap.
/// </summary>
public class Sitemap
{
    /// <summary>
    /// Generates the sitemap for the given context.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task Generate(HttpContext context)
    {
        await context.Response.WriteAsync("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n");
        await context.Response.WriteAsync("<urlset xmlns=\"https://www.sitemaps.org/schemas/sitemap/0.9\">\n");

        var pages = Assembly.GetAssembly(typeof(Utilities))!.ExportedTypes.Where(p => p.IsSubclassOf(typeof(ComponentBase)));

        foreach (var page in pages)
        {
            var siteAttribute = page.GetCustomAttribute<SitemapAttribute>();
            var routeAttribute = page.GetCustomAttribute<RouteAttribute>();

            if (routeAttribute != null)
            {
                if (siteAttribute != null)
                {
                    await context.Response.WriteAsync($@"  <url>
    <loc>https://{context.Request.Host}{routeAttribute.Template}</loc>
    <lastmod>{PackageInformation.BuildDateStringSortable}</lastmod>
    <changefreq>{siteAttribute.ChangeFreq.ToString().ToLower()}</changefreq>
    <priority>{siteAttribute.Priority:N1}</priority>
  </url>{"\n"}");
                }
                else
                {
                    throw new ArgumentNullException($"{page.GetType().Name} with route {routeAttribute.Template} is missing a mandatory {typeof(SitemapAttribute).Name}");
                }
            }
        }

        await context.Response.WriteAsync("</urlset>");
    }
}