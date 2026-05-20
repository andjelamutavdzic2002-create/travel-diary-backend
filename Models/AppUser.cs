using Google.Cloud.Firestore;

namespace TravelDiaryApi.Models;

[FirestoreData]
public class AppUser
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;
    [FirestoreProperty] public string FullName { get; set; } = string.Empty;
    [FirestoreProperty] public string Email { get; set; } = string.Empty;
    [FirestoreProperty] public string PasswordHash { get; set; } = string.Empty;
    [FirestoreProperty] public string Role { get; set; } = "user";
}
