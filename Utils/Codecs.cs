using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Authentication;

using Newtonsoft.Json;

namespace alma.Utils;

/// <summary>
/// A utility class for encoding and decoding base64.
/// </summary>
public class Base64 {

    /// <summary>
    /// Encodes a byte array to a base64 string.
    /// </summary>
    /// <param name="input">Byte array to encode</param>
    /// <returns>Base64 encoded string</returns>
    public static string Encode(byte[] input) {
        return Convert.ToBase64String(input);
    }

    /// <summary>
    /// Encodes a string to a base64 string.
    /// </summary>
    /// <param name="input">String to encode, will be decoded using UTF-8</param>
    /// <returns>Base64 encoded string</returns>
    public static string Encode(string input) {
        return Encode(System.Text.Encoding.UTF8.GetBytes(input));
    }

    /// <summary>
    /// Decodes a base64 string to a byte array.
    /// </summary>
    /// <param name="input">Base64 string to decode</param>
    /// <returns>Decoded byte array</returns>
    public static byte[] Decode(string input) {
        return Convert.FromBase64String(input);
    }

    /// <summary>
    /// Decodes a base64 string to a string.
    /// </summary>
    /// <param name="input">Base64 string to decode</param>
    /// <returns>Decoded data encoded to string using UTF-8</returns>
    public static string DecodeToString(string input) {
        return System.Text.Encoding.UTF8.GetString(Decode(input));
    }
}

/// <summary>
/// A utility class for encoding and decoding base64url.
/// Which are base64 with URL and filename safe alphabets.
/// </summary>
public class Base64Url {

    /// <summary>
    /// Encodes a byte array to a base64url string.
    /// </summary>
    /// <param name="input">Byte array to encode</param>
    /// <returns>Base64url string</returns>
    public static string Encode(byte[] input) {
        return Base64UrlTextEncoder.Encode(input);
    }

    /// <summary>
    /// Encodes a string to a base64url string.
    /// </summary>
    /// <param name="input">String to encode, will be decoded using UTF-8</param>
    /// <returns>Base64url encoded string</returns>
    public static string Encode(string input) {
        return Encode(System.Text.Encoding.UTF8.GetBytes(input));
    }

    /// <summary>
    /// Decodes a base64url string to a byte array.
    /// </summary>
    /// <param name="input">Base64url string to decode</param>
    /// <returns>Decoded byte array</returns>
    public static byte[] Decode(string input) {
        return Base64UrlTextEncoder.Decode(input);
    }

    /// <summary>
    /// Decodes a base64url string to a string.
    /// </summary>
    /// <param name="input">Base64url string to decode</param>
    /// <returns>Decoded data encoded to string using UTF-8</returns>
    public static string DecodeToString(string input) {
        return System.Text.Encoding.UTF8.GetString(Decode(input));
    }
}

/// <summary>
/// A utility class for encoding and decoding URL/percent encoding.
/// </summary>
public class UrlEncoder {

    /// <summary>
    /// Encodes a string to a URL encoded string.
    /// </summary>
    /// <param name="input">String to encode</param>
    /// <returns>URL encoded string</returns>
    public static string Encode(string input) {
        return System.Web.HttpUtility.UrlEncode(input);
    }

    /// <summary>
    /// Decodes a URL encoded string to a string.
    /// </summary>
    /// <param name="input">URL encoded string to decode</param>
    /// <returns>Decoded string</returns>
    public static string Decode(string input) {
        return System.Web.HttpUtility.UrlDecode(input);
    }
}

/// <summary>
/// A utility class for serializing and deserializing JSON objects.
/// </summary>
public class Json {

    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    /// <param name="obj">Object to serialize</param>
    /// <returns>JSON string</returns>
    public static string Serialize(object obj) {
        return System.Text.Json.JsonSerializer.Serialize(obj);
    }

    /// <summary>
    /// Deserializes a JSON string to an object.
    /// </summary>
    /// <param name="json">JSON string to deserialize</param>
    /// <typeparam name="T">Type of the object to deserialize to</typeparam>
    /// <returns>Deserialized object</returns>
    public static T Deserialize<T>(string json) {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json)!;
    }
}

public record class Data() {
    public required byte[] Bytes { get; init; }
    public required string Type { get; init; }
}

/// <summary>
/// A utility class for parsing and creating data URLs.
/// </summary>
public class DataUrl {

    /// <summary>
    /// Parses a data URL to a tuple of data and type
    /// </summary>
    /// <param name="dataUrl">Data URL to parse</param>
    /// <returns>A Data instance containing the bytes and the type</returns>
    public static Data Parse(string dataUrl) {
        var match = Regex.Match(dataUrl, @"data:(?<type>.+?);base64,(?<data>.+)");
        if (!match.Success) {
            throw new ArgumentException("Invalid data URL format.");
        }
        return new Data {
            Bytes = Base64.Decode(match.Groups["data"].Value),
            Type = match.Groups["type"].Value
        };
    }

    /// <summary>
    /// Creates a data URL from a type and data
    /// </summary>
    /// <param name="data">Data to encode</param>
    /// <param name="type">Type of the data</param>
    /// <returns>Data URL</returns>
    public static string Create(byte[] data, string type) {
        return $"data:{type};base64,{Base64.Encode(data)}";
    }
}

/// <summary>
/// A utility class for encoding and decoding HTML.
/// </summary>
public class HtmlEncoder {

    /// <summary>
    /// Encodes a string to an HTML encoded string.
    /// </summary>
    /// <param name="input">String to encode</param>
    /// <returns>HTML encoded string</returns>
    public static string Encode(string input) {
        return System.Web.HttpUtility.HtmlEncode(input);
    }

    /// <summary>
    /// Decodes an HTML encoded string to a string.
    /// </summary>
    /// <param name="input">HTML encoded string to decode</param>
    /// <returns>Decoded string</returns>
    public static string Decode(string input) {
        return System.Web.HttpUtility.HtmlDecode(input);
    }
}
