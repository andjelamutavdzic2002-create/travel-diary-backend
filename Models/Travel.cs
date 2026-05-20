using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace TravelDiaryApi.Models;

[FirestoreData]
public class Travel
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty]
    public string UserId { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Place { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Country { get; set; } = string.Empty;

    [FirestoreProperty]
    public string VisitDate { get; set; } = string.Empty;

    [FirestoreProperty]
    public string Description { get; set; } = string.Empty;

    [FirestoreProperty]
    public int Rating { get; set; }

    [FirestoreProperty]
    public List<string> Images { get; set; } = new();

    [FirestoreProperty]
    public bool IsFavorite { get; set; } = false;
}