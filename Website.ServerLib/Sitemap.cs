using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Website.Lib;
public class Sitemap
{
    public static async Task Generate(HttpContext context)
    {
        await context.Response.WriteAsync("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n");
        await context.Response.WriteAsync("<urlset xmlns=\"https://www.sitemaps.org/schemas/sitemap/0.9\">\n");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var pages = Assembly.GetAssembly(typeof(Utilities)).ExportedTypes.Where(p => p.IsSubclassOf(typeof(ComponentBase)));
#pragma warning restore CS8602 // Dereference of a possibly null reference.

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
                    throw new ArgumentNullException($"{page.GetType().Name} with route {routeAttribute.Template} is missing an mandatory {typeof(SitemapAttribute).Name}");
                }
            }
        }

        await context.Response.WriteAsync("</urlset>");
    }
}