using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotesMicroService.Data;
using NotesMicroService.Models;
using NotesMicroService.DTOs;
using NotesMicroService.Repositories;
using NotesMicroService.Services;

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

// Configure to listen on all network interfaces
builder.WebHost.UseUrls("http://0.0.0.0:80");

// Register PatientsService HTTP client
builder.Services.AddHttpClient("PatientsService", client =>
{
    client.BaseAddress = new Uri("http://patientsmicroservice:80/");
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
