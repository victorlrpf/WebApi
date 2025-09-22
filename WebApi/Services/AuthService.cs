using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendExample.Models;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;

namespace BackendExample.Services;

public class AuthService : IAuthService
{
    private readonly ISupabaseService _db;
    private readonly IConfiguration _config;

    public AuthService(ISupabaseService db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<UserRecord?> RegisterAsync(RegisterRequest request)
    {
        var existing = await _db.GetUserByEmailAsync(request.Email);
        if (existing != null) return null; // already exists

        var user = new UserRecord
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            CreatedAt = DateTime.UtcNow
        };

        return await _db.CreateUserAsync(user);
    }

    public async Task<string?> LoginAsync(LoginRequest request)
    {
        var user = await _db.GetUserByEmailAsync(request.Email);
        if (user == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return null;

        // create JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = _config["Jwt:Secret"]!;
        var key = Encoding.ASCII.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}