using Google.Cloud.Firestore;
using TravelDiaryApi.DTOs;
using TravelDiaryApi.Models;

namespace TravelDiaryApi.Services;

public class TravelService
{
    private readonly FirestoreDb _db;

    public TravelService(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<List<Travel>> GetUserTravelsAsync(string userId)
    {
        var snapshot = await _db
            .Collection("travels")
            .WhereEqualTo("UserId", userId)
            .GetSnapshotAsync();

        return snapshot.Documents.Select(d =>
        {
            var travel = d.ConvertTo<Travel>();
            travel.Id = d.Id;
            return travel;
        }).ToList();
    }

    public async Task<Travel?> GetByIdAsync(string id, string userId)
    {
        var doc = await _db
            .Collection("travels")
            .Document(id)
            .GetSnapshotAsync();

        if (!doc.Exists)
            return null;

        var travel = doc.ConvertTo<Travel>();
        travel.Id = doc.Id;

        return travel.UserId == userId ? travel : null;
    }

    public async Task<Travel> CreateAsync(TravelDto dto, string userId)
    {
        var travel = new Travel
        {
            UserId = userId,
            Place = dto.Place,
            Country = dto.Country,
            VisitDate = dto.VisitDate,
            Description = dto.Description,
            Rating = dto.Rating,
            Images = dto.Images,
            IsFavorite = dto.IsFavorite
        };

        var doc = await _db
            .Collection("travels")
            .AddAsync(travel);

        travel.Id = doc.Id;

        return travel;
    }

    public async Task<bool> UpdateAsync(string id, TravelDto dto, string userId)
    {
        if (await GetByIdAsync(id, userId) == null)
            return false;

        var updated = new Travel
        {
            Id = id,
            UserId = userId,
            Place = dto.Place,
            Country = dto.Country,
            VisitDate = dto.VisitDate,
            Description = dto.Description,
            Rating = dto.Rating,
            Images = dto.Images,
            IsFavorite = dto.IsFavorite
        };

        await _db
            .Collection("travels")
            .Document(id)
            .SetAsync(updated);

        return true;
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        if (await GetByIdAsync(id, userId) == null)
            return false;

        await _db
            .Collection("travels")
            .Document(id)
            .DeleteAsync();

        return true;
    }
}