namespace Website.Lib;

/// <summary>
/// Produces a nonce and manages static asset SHA hashes
/// for use with CSP.
/// </summary>
public class ContentSecurityPolicyService
{
    /// <summary>
    /// The Scoped nonce value.
    /// </summary>
    public readonly string NonceValue = "";


    public readonly string ScriptSrc = "'self'";


    public ContentSecurityPolicyService()
    {
        var bytes = new byte[32];

        var rnd = new Random();

        rnd.NextBytes(bytes);

        NonceValue = Convert.ToBase64String(bytes);

        var hashesFilePath = AppContext.BaseDirectory + "hashes.csv";

        if (!File.Exists(hashesFilePath))
        {
            return;
        }

        using StreamReader sr = new(hashesFilePath);

        string str = "";

        while (sr.Peek() >= 0)
        {
            var csvSplit = (sr.ReadLine() ?? ",").Split(',');

            var extension = csvSplit[0].Split('.')[^1].ToLower();

            if (extension == "js")
            {
                str = str + $"'sha256-{csvSplit[1]}' ";
            }
        }

        ScriptSrc = str.Trim();
    }
}
