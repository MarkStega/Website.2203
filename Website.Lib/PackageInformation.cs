using System.Reflection;

namespace Website.Lib;


/// <summary>
/// Supplies information about the assembly/package. 
/// </summary>
public static class PackageInformation
{
    static PackageInformation()
    {
        var versionString = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "";

        if (string.IsNullOrWhiteSpace(versionString))
        {
            Version = "0.0.0";
            BuildTime = DateTime.UtcNow;
        }
        else
        {
            var parts = versionString.Split("--");
            Version = parts[2];

            var dateParts = GetParts(parts[0], "-");
            var timeParts = GetParts(parts[1], "-");

            BuildTime = new(dateParts[0], dateParts[1], dateParts[2], timeParts[0], timeParts[1], timeParts[2], DateTimeKind.Utc);
        }

        BuildTimeString = BuildTime.ToString("G");

        var versionParts = GetParts(Version, ".");
        MajorVersion = versionParts[0];
        MinorVersion = versionParts[1];
        Patch = versionParts[2];

        var buildDateString = Assembly.GetExecutingAssembly().GetCustomAttribute<BuildDateAttribute>()?.DateString ?? "";

        if (string.IsNullOrWhiteSpace(buildDateString))
        {
            BuildDate = DateOnly.FromDateTime(DateTime.UtcNow);
        }
        else
        {
            var dateParts = GetParts(buildDateString, "-");
            BuildDate = new(dateParts[0], dateParts[1], dateParts[2]);
        }

        BuildDateString = BuildDate.ToString("d");
        BuildDateStringSortable = BuildDate.ToString("yyyy-MM-dd");
    }


    /// <summary>
    /// Returns a string with the value of the InformationalVersion
    /// </summary>
    /// <returns></returns>
    public static readonly string Version;


    /// <summary>
    /// SEMVER major version number
    /// </summary>
    /// <returns></returns>
    public static readonly int MajorVersion;


    /// <summary>
    /// SEMVER minor version number
    /// </summary>
    /// <returns></returns>
    public static readonly int MinorVersion;


    /// <summary>
    /// SEMVER patch number
    /// </summary>
    /// <returns></returns>
    public static readonly int Patch;


    /// <summary>
    /// The build date
    /// </summary>
    /// <returns></returns>
    public static readonly DateOnly BuildDate;


    /// <summary>
    /// The build date
    /// </summary>
    /// <returns></returns>
    public static readonly string BuildDateString;


    /// <summary>
    /// The build date in "yyyy-MM-dd" format
    /// </summary>
    /// <returns></returns>
    public static readonly string BuildDateStringSortable;


    /// <summary>
    /// The build date and time
    /// </summary>
    /// <returns></returns>
    public static readonly DateTime BuildTime;


    /// <summary>
    /// Teh build time as a string
    /// </summary>
    /// <returns></returns>
    public static readonly string BuildTimeString;


    private static int[] GetParts(string dateOrTime, string separator) => dateOrTime.Split(separator).Select(x => Convert.ToInt32(x)).ToArray();
}
