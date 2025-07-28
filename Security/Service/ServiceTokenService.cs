using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Security.Service;

/// <summary>
/// Service for generating JWT tokens for inter-service communication, across microservices.
/// This is used to authenticate requests between services in a microservices architecture.
/// </summary>
public class ServiceTokenService
{
    private readonly string _secretKey;
    private readonly string _issuer;

    public ServiceTokenService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:ServiceSecretKey"] ?? throw new ArgumentNullException("Jwt:ServiceSecretKey");
        _issuer = configuration["Jwt:Issuer"] ?? "SecurityService";
    }

    public string GenerateServiceToken(string targetService, string userId, IList<string> userRoles, DateTime? expiration = null)
    {
        var claims = new List<Claim>
        {
            new("service", "SecurityService"),
            new("target", targetService),
            new("user_id", userId),
            new("timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        // Add user roles for authorization
        claims.AddRange(userRoles.Select(role => new Claim("user_role", role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: targetService,
            claims: claims,
            expires: expiration ?? DateTime.UtcNow.AddMinutes(5), // Short lived for security
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
