using Microsoft.EntityFrameworkCore;
using NotesMicroService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure to listen on all interfaces
builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

// Replace this section in Program.cs:
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var retryCount = 0;
    var maxRetries = 15;
    var delay = TimeSpan.FromSeconds(5);

    while (retryCount < maxRetries)
    {
        try
        {
            logger.LogInformation("Attempting to connect to database (attempt {Attempt}/{MaxRetries})",
                retryCount + 1, maxRetries);

            await dbContext.Database.CanConnectAsync();

            // Use Migrate() instead of EnsureCreatedAsync()
            await dbContext.Database.MigrateAsync();

            logger.LogInformation("Database migration successful");
            break;
        }
        catch (Exception ex)
        {
            retryCount++;
            logger.LogWarning("Database migration failed (attempt {Attempt}/{MaxRetries}): {Error}",
                retryCount, maxRetries, ex.Message);

            if (retryCount >= maxRetries)
            {
                logger.LogError("Max database migration retries exceeded. Application will exit.");
                throw;
            }

            await Task.Delay(delay);
        }
    }
}


app.Run();