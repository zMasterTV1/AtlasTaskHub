using Atlas.Application.Abstractions.Security;
using System.Security.Cryptography;

namespace Atlas.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 120_000;

    public (string hash, string salt) HashPassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(KeySize);

        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public bool VerifyPassword(string password, string expectedHash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var expectedBytes = Convert.FromBase64String(expectedHash);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, HashAlgorithmName.SHA256);
        var actualBytes = pbkdf2.GetBytes(KeySize);

        return CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
    }
}
