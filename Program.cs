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
var firebaseCredentialsPath = builder.Configuration["Firebase:CredentialsPath"];

if (!string.IsNullOrWhiteSpace(firebaseCredentialsPath) && File.Exists(firebaseCredentialsPath))
{
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseCredentialsPath);
    FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromFile(firebaseCredentialsPath) });
}

builder.Services.AddSingleton(_ =>
{
    var credential = GoogleCredential.FromFile(firebaseCredentialsPath);

    return new FirestoreDbBuilder
    {
        ProjectId = firebaseProjectId,
        Credential = credential
    }.Build();
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TravelService>();

var jwtKey = builder.Configuration["Jwt:Key"]!;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

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
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
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
