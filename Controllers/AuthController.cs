using Google.Api;
using Microsoft.AspNetCore.Mvc;
using TravelDiaryApi.DTOs;
using TravelDiaryApi.Services;

namespace TravelDiaryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                dto.Password.Length < 6)
            {
                return BadRequest("Ime, email i lozinka od najmanje 6 karaktera su obavezni.");
            }

            var result = await _authService.RegisterAsync(dto);

            if (result == null)
            {
                return BadRequest("Korisnik sa ovom email adresom već postoji.");
            }

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Greška na serveru prilikom registracije.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Email i lozinka su obavezni.");
            }

            var result = await _authService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized("Pogrešan email ili lozinka.");
            }

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Greška na serveru prilikom prijave.");
        }
    }
}