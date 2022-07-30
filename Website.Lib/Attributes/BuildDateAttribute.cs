namespace Website.Lib;


[AttributeUsage(AttributeTargets.Assembly)]
public class BuildDateAttribute : Attribute
{
    public readonly string DateString;

    public BuildDateAttribute(string dateString)
    {
        DateString = dateString;
    }
}