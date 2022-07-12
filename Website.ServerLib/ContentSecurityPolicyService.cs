using Microsoft.AspNetCore.Hosting;

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


    public string ScriptSrc { get; private set; } = $"'self'";


    public ContentSecurityPolicyService(IWebHostEnvironment env)
    {
        var bytes = new byte[32];

        var rnd = new Random();

        rnd.NextBytes(bytes);

        NonceValue = Convert.ToBase64String(bytes);

        var hashesFilePath = AppContext.BaseDirectory + "hashes.csv";

        string str = "";

        if (File.Exists(hashesFilePath))
        {
            using StreamReader sr = new(hashesFilePath);

            while (sr.Peek() >= 0)
            {
                var csvSplit = (sr.ReadLine() ?? ",").Split(',');

                var extension = csvSplit[0].Split('.')[^1].ToLower();

                if (extension == "js")
                {
                    str += $"'sha256-{csvSplit[1]}' ";
                }
            }
        }
#if DEBUG
        else
        {
            var rootPath = env.WebRootPath;
        }
#endif

        var hexString = "C02FB30326075533737AF0B0DD216F1C8E231B9D69575F9BE6C437463D754062";
        var base64String = Convert.ToBase64String(Convert.FromHexString(hexString));

        str += $"'sha256-{base64String}' ";

        ScriptSrc = ($"'nonce-{NonceValue}' " + str).Trim();
    }
}
