using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Google.Cloud.Firestore;
using Microsoft.IdentityModel.Tokens;
using TravelDiaryApi.DTOs;
using TravelDiaryApi.Models;

namespace TravelDiaryApi.Services;

public class AuthService
{
    private readonly FirestoreDb _db;
    private readonly IConfiguration _config;

    public AuthService(FirestoreDb db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var existing = await _db.Collection("users").WhereEqualTo("Email", dto.Email).GetSnapshotAsync();
        if (existing.Documents.Count > 0) return null;
        var user = new AppUser { FullName = dto.FullName, Email = dto.Email, PasswordHash = HashPassword(dto.Password), Role = "user" };
        var doc = await _db.Collection("users").AddAsync(user);
        user.Id = doc.Id;
        return CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var snapshot = await _db.Collection("users").WhereEqualTo("Email", dto.Email).GetSnapshotAsync();
        var document = snapshot.Documents.FirstOrDefault();
        if (document == null) return null;
        var user = document.ConvertTo<AppUser>();
        user.Id = document.Id;
        if (user.PasswordHash != HashPassword(dto.Password)) return null;
        return CreateAuthResponse(user);
    }

    private AuthResponseDto CreateAuthResponse(AppUser user)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id), new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.Name, user.FullName), new Claim(ClaimTypes.Role, user.Role) };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: credentials);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthResponseDto(jwt, user.Id, user.FullName, user.Email, user.Role);
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
