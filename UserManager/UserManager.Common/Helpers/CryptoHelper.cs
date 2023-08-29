using System.Security.Cryptography;
using System.Text;

namespace UserManager.Common.Helpers;

public static class CryptoHelper
{
    private const int SaltLength = 32;
    public static string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();

        var salt = new byte[SaltLength];
        rng.GetBytes(salt);

        return Convert.ToBase64String(salt);
    }

    public static bool ConfirmPassword(string password, string salt, string hashedPassword)
    {
        var newHashPassword = GetHash(password, salt);
        return string.Equals(hashedPassword, newHashPassword, StringComparison.InvariantCulture);
    }

    public static string GetHash(string inputString, string salt = "")
    {
        using var sha256 = SHA256.Create();
        inputString = string.Concat(inputString, salt);

        return GetHash(sha256, inputString);
    }

    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {
        var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sBuilder = new StringBuilder();

        foreach (var symbol in data)
            sBuilder.Append(symbol.ToString("x2"));

        return sBuilder.ToString();
    }

    public static string GenerateToken()
    {
        return GenerateSalt();
    }
}
