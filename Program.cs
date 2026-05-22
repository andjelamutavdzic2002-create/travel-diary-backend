using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TravelDiaryApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var firebaseProjectId = builder.Configuration["Firebase:ProjectId"];
var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");

builder.Services.AddSingleton(_ =>
{
    GoogleCredential credential;

    if (!string.IsNullOrWhiteSpace(firebaseJson))
    {
        credential = GoogleCredential.FromJson(firebaseJson);
    }
    else
    {
        var firebaseCredentialsPath = builder.Configuration["Firebase:CredentialsPath"];
        credential = GoogleCredential.FromFile(firebaseCredentialsPath);
    }

    if (FirebaseApp.DefaultInstance == null)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = credential
        });
    }

    return new FirestoreDbBuilder
    {
        ProjectId = firebaseProjectId,
        Credential = credential
    }.Build();
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TravelService>();

var jwtKey = builder.Configuration["Jwt:Key"];
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = key
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();