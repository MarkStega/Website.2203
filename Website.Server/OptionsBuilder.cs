using HttpSecurity.AspNet;

namespace Website.Server;


/// <summary>
/// Builds options for ASP.NET services;
/// </summary>
public static class OptionsBuilder
{
    /// <summary>
    /// Builds headers that are applied generally at the start of middleware. These may get overridden by subsequent middleware.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static HttpSecurityOptions BuildGeneralHeaderOptions(WebApplicationBuilder builder, HttpSecurityOptions options)
    {
        return options

        .AddContentSecurityOptions(cspOptions =>
        {
            cspOptions
            .AddBaseUri(o => o.AddSelf())
            .AddBlockAllMixedContent()
            .AddChildSrc(o => o.AddSelf())
            .AddConnectSrc(o => o.AddSelf().AddUri((baseUri, baseDomain) => $"wss://{baseDomain}:*").AddUri("https://www.googletagmanager.com").AddUri("https://www.google-analytics.com").AddUri("https://region1.google-analytics.com").AddUri("https://p.typekit.net").AddUri("https://use.typekit.net").AddUri("https://fonts.googleapis.com").AddUri("https://fonts.gstatic.com"))
            .AddDefaultSrc(o => o.AddSelf())
            .AddFontSrc(o => o.AddUri("https://use.typekit.net").AddUri("https://fonts.googleapis.com").AddUri("https://fonts.gstatic.com"))
            .AddFrameAncestors(o => o.AddNone())
            .AddFrameSrc(o => o.AddSelf())
            .AddFormAction(o => o.AddNone())
            .AddImgSrc(o => o.AddSelf().AddUri("https://www.google-analytics.com").AddUri("https://*.openstreetmap.org").AddSchemeSource(SchemeSource.Data, "w3.org/svg/2000"))
            .AddManifestSrc(o => o.AddSelf())
            .AddMediaSrc(o => o.AddSelf())
            .AddObjectSrc(o => o.AddNone())
            .AddReportUri(o => o.AddUri((baseUri, baseDomain) => $"https://{baseUri}/api/CspReporting/UriReport"))
            // The sha-256 hash relates to an inline script added by blazor's javascript
            .AddScriptSrc(o =>
                    o.AddHashValue(HashAlgorithm.SHA256, "v8v3RKRPmN4odZ1CWM5gw80QKPCCWMcpNeOmimNL2AA=")
                    .AddUriIf((baseUri, baseDomain) => $"https://{baseUri}/_framework/aspnetcore-browser-refresh.js", () => builder.Environment.IsDevelopment())
                    .AddSelfIf(() => builder.Environment.IsDevelopment() || PlatformDetermination.IsBlazorWebAssembly)
                    //.AddStrictDynamicIf(() => !builder.Environment.IsDevelopment() && PlatformDetermination.IsBlazorWebAssembly) // this works on Chromium browswers but fails for both Firefox and Safari
                    .AddUnsafeInlineIf(() => PlatformDetermination.IsBlazorWebAssembly)
                    .AddReportSample()
                    .AddUnsafeEvalIf(() => PlatformDetermination.IsBlazorWebAssembly)
                    .AddUri("https://www.googletagmanager.com/gtag/js")
                    .AddUri((baseUri, baseDomain) => $"https://{baseUri}/_content/GoogleAnalytics.Blazor/googleanalytics.blazor.js") // Required to work on Safari
                    .AddUri((baseUri, baseDomain) => $"https://{baseUri}/_content/Material.Blazor/material.blazor.min.js") // Required to work on Safari
                    .AddUri((baseUri, baseDomain) => $"https://{baseUri}/_content/Website.Client/js/dioptra.min.js") // Required to work on Safari
                    .AddUriIf((baseUri, baseDomain) => $"https://{baseUri}/_framework/blazor.server.js", () => PlatformDetermination.IsBlazorServer) // Required to work on Safari
                    .AddUriIf((baseUri, baseDomain) => $"https://{baseUri}/_framework/blazor.webassembly.js", () => PlatformDetermination.IsBlazorWebAssembly) // Required to work on Safari
                    .AddGeneratedHashValues(StaticFileExtension.JS))
            .AddStyleSrc(o => o.AddSelf().AddUnsafeInline().AddReportSample().AddUri("https://p.typekit.net").AddUri("https://use.typekit.net").AddUri("https://fonts.googleapis.com").AddUri("https://fonts.gstatic.com"))
            .AddUpgradeInsecureRequests()
            .AddWorkerSrc(o => o.AddSelf());
        })
        // ref: <a href="http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers">http://stackoverflow.com/questions/49547/making-sure-a-web-page-is-not-cached-across-all-browsers</a>
        .AddReferrerPolicy(ReferrerPolicyDirective.NoReferrer)
        .AddPermissionsPolicy("accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()")
        .AddStrictTransportSecurity(31536000, true)
        .AddXClientId("Dioptra")
        .AddXContentTypeOptionsNoSniff()
        .AddXFrameOptionsDirective(XFrameOptionsDirective.Deny)
        .AddXXssProtectionDirective(XXssProtectionDirective.OneModeBlock)
        .AddXPermittedCrossDomainPoliciesDirective(XPermittedCrossDomainPoliciesDirective.None);
    }


    /// <summary>
    /// Builds headers that are applied at the start of sending the <see cref="HttpResponse"/>, and once middleware is complete.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public static HttpSecurityOptions BuildOnStartupHeaderOptions(WebApplicationBuilder builder, HttpSecurityOptions options)
    {
        return options

        .AddCacheControl("no-cache, public, max-age=86400")
        .AddExpires("0");
    }
}
