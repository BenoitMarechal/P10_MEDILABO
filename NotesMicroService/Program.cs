using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotesMicroService.Data;
using NotesMicroService.Models;
using NotesMicroService.DTOs;
using NotesMicroService.Repositories;
using NotesMicroService.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB settings from appsettings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Register MongoClient singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Register Notes repository
builder.Services.AddSingleton<NotesRepository>();

// Register controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("M2U2NmMzMjQtNGIwOS00ZWVmLWIzOGYtZTYzZDE3ZjZkZGJkMzM1MGIzNmEtOWI4OC00ZTVjLThiZWMtNjhlZGVlMTg5YmJi")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

// Configure to listen on all network interfaces
builder.WebHost.UseUrls("http://0.0.0.0:80");

builder.Services.AddHttpContextAccessor();

// Register PatientsService HTTP client
builder.Services.AddHttpClient("PatientsService", client =>
{
    client.BaseAddress = new Uri("http://patientsmicroservice:80/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});


builder.Services.AddScoped<PatientsService>();

var app = builder.Build();

// Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Authorization & routing
app.UseAuthorization();
app.MapControllers();

// Optional: test MongoDB connection at startup
using (var scope = app.Services.CreateScope())
{
    var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Ping the database to ensure connection works
        var settings = scope.ServiceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
        var database = mongoClient.GetDatabase(settings.Database);
        database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}").Wait();

        logger.LogInformation("Connected to MongoDB successfully");
    }
    catch (Exception ex)
    {
        logger.LogError("Failed to connect to MongoDB: {Error}", ex.Message);
        throw; // fail fast if MongoDB is unreachable
    }
}

app.Run();
