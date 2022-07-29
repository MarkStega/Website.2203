using System.Reflection;
using System.Linq;

namespace Website.Lib;


/// <summary>
/// Supplies information about the assembly/package. 
/// </summary>
public static class PackageInformation
{
    static PackageInformation()
    {
        Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "";
        BuildTime = DateTime.UtcNow;

        var bt = Environment.GetEnvironmentVariable("BUILD_TIME") ?? "";

        var parts = bt.Split("-");

        if (parts.Length == 7)
        {
            parts[3] = "0";

            var intParts = parts.Select(x => Convert.ToInt32(x)).ToArray();

            BuildTime = new(intParts[0], intParts[1], intParts[2], intParts[4], intParts[5], intParts[6]);
        }

        BuildTimeString = BuildTime.ToShortDateString() + " " + BuildTime.ToShortTimeString() + " UTC";
    }

    /// <summary>
    /// Returns a string with the value of the InformationalVersion
    /// </summary>
    /// <returns></returns>
    public static readonly string Version;


    /// <summary>
    /// The build time
    /// </summary>
    /// <returns></returns>
    public static readonly DateTime BuildTime;


    /// <summary>
    /// Teh build time as a string
    /// </summary>
    /// <returns></returns>
    public static readonly string BuildTimeString;
}
