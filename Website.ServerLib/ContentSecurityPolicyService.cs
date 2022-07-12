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


    /// <summary>
    /// Part of the CSP <c>script-src</c> tag that encodes sha keys and nonce values.
    /// </summary>
    public string ScriptSrcPart { get; private set; } = $"'self'";


    /// <summary>
    /// Part of the CSP <c>style-src</c> tag that encodes sha keys and nonce values.
    /// </summary>
    public string StyleSrcPart { get; private set; } = $"'self'";


    private Dictionary<string, string> fileHashes = new();


    public ContentSecurityPolicyService()
    {
        var bytes = new byte[32];

        var rnd = new Random();

        rnd.NextBytes(bytes);

        NonceValue = Convert.ToBase64String(bytes);

        var hashesFilePath = AppContext.BaseDirectory + "hashes.csv";

        string scriptSrcPart = "";
        string styleSrcPart = "";

        if (File.Exists(hashesFilePath) && !fileHashes.Any())
        {
            using StreamReader sr = new(hashesFilePath);

            while (sr.Peek() >= 0)
            {
                var csvSplit = (sr.ReadLine() ?? ",").Split(',');

                var fileName = csvSplit[0].Split('\\')[^1];
                var extension = csvSplit[0].Split('.')[^1].ToLower();
                var hashString = $"sha256-{csvSplit[1]}";

                fileHashes[fileName] = hashString;

                if (extension == "js")
                {
                    scriptSrcPart += $"'sha256-{csvSplit[1]}' ";
                }
                else if (extension == "css")
                {
                    styleSrcPart += $"'sha256-{csvSplit[1]}' ";
                }
            }
        }

        //var hexString1 = "C02FB30326075533737AF0B0DD216F1C8E231B9D69575F9BE6C437463D754062";
        //var hexString2 = "c02fb30326075533737af0b0dd216f1c8e231b9d69575f9be6c437463d754062";
        //var base64String = Convert.ToBase64String(Convert.FromHexString(hexString2));

        //str += $"'sha256-{base64String}' 'sha256-wC+zAyYHVTNzevCw3SFvHI4jG51pV1+b5sQ3Rj11QGI=' ";

        ScriptSrcPart = ($"'nonce-{NonceValue}' " + scriptSrcPart).Trim();
        StyleSrcPart = ($"'nonce-{NonceValue}' " + styleSrcPart).Trim();
    }


    /// <summary>
    /// Returns the hash string for a given file name to be used in a script of style's <c>integrity="[value]"</c> tag.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public string GetFileHashString(string fileName)
    {
        return fileHashes[fileName];
    }
}
