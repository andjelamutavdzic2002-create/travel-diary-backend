using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelDiaryApi.DTOs;
using TravelDiaryApi.Services;

namespace TravelDiaryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TravelsController : ControllerBase
{
    private readonly TravelService _travelService;

    public TravelsController(TravelService travelService)
    {
        _travelService = travelService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var travels = await _travelService.GetUserTravelsAsync(UserId);
            return Ok(travels);
        }
        catch (Exception)
        {
            return StatusCode(500, "Greška pri učitavanju putovanja.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("ID putovanja je obavezan.");
            }

            var travel = await _travelService.GetByIdAsync(id, UserId);

            if (travel == null)
            {
                return NotFound("Putovanje nije pronađeno.");
            }

            return Ok(travel);
        }
        catch (Exception)
        {
            return StatusCode(500, "Greška pri učitavanju putovanja.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(TravelDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Place) ||
                string.IsNullOrWhiteSpace(dto.Country))
            {
                return BadRequest("Mesto i država su obavezni.");
            }

            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return BadRequest("Ocena mora biti od 1 do 5.");
            }

            var created = await _travelService.CreateAsync(dto, UserId);

            return Ok(created);
        }
        catch (Exception)
        {
            return StatusCode(500, "Greška pri kreiranju putovanja.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, TravelDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("ID putovanja je obavezan.");
            }

            if (string.IsNullOrWhiteSpace(dto.Place) ||
                string.IsNullOrWhiteSpace(dto.Country))
            {
                return BadRequest("Mesto i država su obavezni.");
            }

            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return BadRequest("Ocena mora biti od 1 do 5.");
            }

            var ok = await _travelService.UpdateAsync(id, dto, UserId);

            if (!ok)
            {
                return NotFound("Putovanje nije pronađeno.");
            }

            return Ok("Putovanje je izmenjeno.");
        }
        catch (Exception)
        {
            return StatusCode(500, "Greška pri izmeni putovanja.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("ID putovanja je obavezan.");
            }

            var ok = await _travelService.DeleteAsync(id, UserId);

            if (!ok)
            {
                return NotFound("Putovanje nije pronađeno.");
            }

            return Ok("Putovanje je obrisano.");
        }
        catch (Exception)
        {
            return StatusCode(500, "Greška pri brisanju putovanja.");
        }
    }
}