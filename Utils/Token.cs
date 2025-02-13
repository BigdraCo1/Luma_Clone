using System.Security.Cryptography;

namespace alma.Utils;

/// <summary>
/// A utility class for generating tokens.
/// Tokens are encoded in base64url
/// with the specified entropy.
/// Tokens length is variable.
/// </summary>
public class Token {

    /// <summary>
    /// Generates a token with the specified entropy.
    /// </summary>
    /// <param name="entropy">Entropy amount in bits</param>
    /// <returns></returns>
    public static string Generate(int entropy) {
        var bytesCount = entropy / 8;
        if (entropy % 8 != 0) {
            bytesCount++;
        }

        byte[] bytes = new byte[bytesCount];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(bytes);
        }

        return Base64Url.Encode(bytes);
    }
}