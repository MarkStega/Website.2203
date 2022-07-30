namespace Website.Lib;

[AttributeUsage(AttributeTargets.Class)]
public class SitemapAttribute : Attribute
{
    public enum ChangeFreqType { Always, Hourly, Daily, Weekly, Monthly, Yearly, Never };

    public ChangeFreqType ChangeFreq = ChangeFreqType.Monthly;
    public double Priority = 0.5;

    public SitemapAttribute(ChangeFreqType changeFreq, double priority)
    {
        if (priority < 0 || priority > 1)
        {
            throw new ArgumentException($"Priority cannot be {priority} - must be between 0 and 1.");
        }

        ChangeFreq = changeFreq;
        Priority = priority;
    }
}
