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
        byte[] bytes = new byte[12];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(bytes);
        }

        return Base64Url.Encode(bytes);
    }

    /// <summary>
    /// Converts an integer string to a TUID.
    /// </summary>
    /// <param name="intString">Integer in the form of string</param>
    /// <returns>TUID as string</returns>
    public static string FromIntString(string intString) {
        BigInteger num = BigInteger.Parse(intString);
        byte[] bytes = num.ToByteArray();

        if (bytes.Length > 12) {
            bytes = bytes[..12];
        }
        if (bytes.Length < 12) {
            byte[] paddedBytes = new byte[12];
            bytes.CopyTo(paddedBytes, 0);
            bytes = paddedBytes;
        }
        bytes = [.. bytes.Reverse()];

        return Base64Url.Encode(bytes);
    }
}