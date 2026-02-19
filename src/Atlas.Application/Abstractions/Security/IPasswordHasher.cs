namespace Atlas.Application.Abstractions.Security;

public interface IPasswordHasher
{
    (string hash, string salt) HashPassword(string password);
    bool VerifyPassword(string password, string expectedHash, string salt);
}
