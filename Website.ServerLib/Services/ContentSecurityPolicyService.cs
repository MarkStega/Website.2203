using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
    /// Formatted nonce string.
    /// </summary>
    public string NonceString => $"'nonce-{NonceValue}'";


    /// <summary>
    /// Part of the CSP <c>script-src</c> tag that encodes sha keys and nonce values.
    /// </summary>
    public readonly string ScriptSrcPart  = "'self'";


    /// <summary>
    /// Part of the CSP <c>style-src</c> tag that encodes sha keys and nonce values.
    /// </summary>
    public readonly string StyleSrcPart = "'self'";


    /// <summary>
    /// The CSP is to be applied only if this is true.
    /// </summary>
    public readonly bool ApplyContentSecurityPolicy = true;


    // Delimiters are for Linux and Windows respectively.
    private static readonly char[] _pathDelimiters = { '/', '\\' };

    private readonly Dictionary<string, string> _fileHashes = new();


    public ContentSecurityPolicyService(IWebHostEnvironment env)
    {
        var bytes = new byte[32];

        var rnd = new Random();

        rnd.NextBytes(bytes);

        NonceValue = Convert.ToBase64String(bytes);

        var hashesFilePath = AppContext.BaseDirectory + "hashes.csv";

        string scriptSrcPathPart = "";
        string scriptSrcHashesPart = "";
        string styleSrcHashesPart = "";

        if (File.Exists(hashesFilePath) && !_fileHashes.Any())
        {
            using StreamReader sr = new(hashesFilePath);

            while (sr.Peek() >= 0)
            {
                var csvSplit = (sr.ReadLine() ?? ",").Split(',');

                var path = csvSplit[0].Split("wwwroot")[^1][1..];
                var fileName = csvSplit[0].Split(_pathDelimiters)[^1];
                var extension = csvSplit[0].Split('.')[^1].ToLower();
                var hashString = $"sha256-{csvSplit[1]}";

                _fileHashes[fileName] = hashString;
                
                if (extension == "js")
                {
                    scriptSrcPathPart += $"'{path}' ";
                    scriptSrcHashesPart += $"'{hashString}' ";
                }
                else if (extension == "css")
                {
                    styleSrcHashesPart += $"'{hashString}' ";
                }
            }
        }

        ApplyContentSecurityPolicy = File.Exists(hashesFilePath) || !env.IsDevelopment();

        ScriptSrcPart = (NonceString + " " + scriptSrcPathPart + " " + scriptSrcHashesPart).Trim();
        StyleSrcPart = (NonceString + " " + styleSrcHashesPart).Trim();
    }


    /// <summary>
    /// Returns the hash string for a given file name to be used in a script of style's <c>integrity="[value]"</c> tag.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public string GetFileHashString(string fileName)
    {
        if (_fileHashes.TryGetValue(fileName, out var hash))
        {
            return hash;
        }

        return "";
    }
}
