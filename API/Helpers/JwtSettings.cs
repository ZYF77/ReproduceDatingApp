using System;

namespace API.Helpers;

public class JwtSettings
{
    public required string Secret { get; set; } = string.Empty;
    public required string Issuer { get; set; } = string.Empty;
    public required string Audience { get; set; } = string.Empty;
    public int TokenExpiryInMinutes { get; set; }
}
