namespace Atlas.Infrastructure.Security;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "AtlasTaskHub";
    public string Audience { get; set; } = "AtlasTaskHub";
    /// <summary>At least 32 chars for HMACSHA256.</summary>
    public string SigningKey { get; set; } = "dev_signing_key_change_me_please_32chars";
    public int AccessTokenMinutes { get; set; } = 30;
    public int RefreshTokenDays { get; set; } = 14;
}
