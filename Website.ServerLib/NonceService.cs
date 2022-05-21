namespace Website.Lib;

/// <summary>
/// Produces a nonce for use with CSP.
/// </summary>
public class NonceService
{
    /// <summary>
    /// The Scoped nonce value.
    /// </summary>
    public readonly string NonceValue = "";

    public NonceService()
    {
        var bytes = new byte[32];

        var rnd = new Random();

        rnd.NextBytes(bytes);

        NonceValue = Convert.ToBase64String(bytes);
    }
}
