namespace alma.Utils;

public class Etag {
    public static string Generate(byte[] data) {
        var hash = System.Security.Cryptography.MD5.HashData(data);
        return Base64Url.Encode(hash);
    }
}
