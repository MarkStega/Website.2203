namespace Website.Server;

/// <summary>
/// A class that determines the current platform as being either Blazor Server or WebAssembly.
/// </summary>
public static class PlatformDetermination
{
#if BLAZOR_SERVER
    /// <summary>
    /// We are running Blazor Server.
    /// </summary>
    public const bool IsBlazorServer = true;
    public const bool IsBlazorWebAssembly = false;
#else
    /// <summary>
    /// We are running Blazor WebAssembly.
    /// </summary>
    public const bool IsBlazorServer = false;
    public const bool IsBlazorWebAssembly = true;
#endif
}
