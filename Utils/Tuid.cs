using System.Numerics;
using System.Security.Cryptography;

namespace alma.Utils;

/// <summary>
/// A utility class for generating TUIDs.
/// = Triple Word Unique Identifier
/// with 96 bits of entropy.
/// Encoded in 16 digits base64url.
/// </summary>
public class Tuid {
    /// <summary>
    /// Generates a TUID.
    /// </summary>
    /// <returns>TUID as string</returns>
    public static string Generate() {
        byte[] RandomBytes = new byte[12];
        using (RandomNumberGenerator Rng = RandomNumberGenerator.Create()) {
            Rng.GetBytes(RandomBytes);
        }
        string Base64 = Convert.ToBase64String(RandomBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
        return Base64[..16];
    }

    /// <summary>
    /// Converts an integer string to a TUID.
    /// </summary>
    /// <param name="IntString">Integer in the form of string</param>
    /// <returns>TUID as string</returns>
    public static string GetFromIntString(string IntString) {
        BigInteger Int = BigInteger.Parse(IntString);
        byte[] Bytes = Int.ToByteArray();
        Bytes = [.. Bytes.Reverse()];
        var Id = Convert.ToBase64String(Bytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
        return Id.PadLeft(16, '0');
    }
}