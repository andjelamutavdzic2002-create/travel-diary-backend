namespace TravelDiaryApi.DTOs;

public record RegisterDto(string FullName, string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token, string UserId, string FullName, string Email, string Role);
