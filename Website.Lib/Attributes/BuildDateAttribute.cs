namespace Website.Lib;

/// <summary>
/// Adds an assembly attribute enabling the build process to pass in the build date.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class BuildDateAttribute : Attribute
{
    /// <summary>
    /// The calculated build date.
    /// </summary>
    public readonly string DateString;

    public BuildDateAttribute(string dateString)
    {
        DateString = dateString;
    }
}