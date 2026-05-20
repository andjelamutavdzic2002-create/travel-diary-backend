using System.Collections.Generic;

namespace TravelDiaryApi.DTOs;

public class TravelDto
{
    public string Place { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string VisitDate { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Rating { get; set; }

    public List<string> Images { get; set; } = new();

    public bool IsFavorite { get; set; } = false;
}