var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddLogging();

// Keep your existing HTTP client configuration
builder.Services.AddHttpClient("Patients", client =>
{
    client.BaseAddress = new Uri("http://apigateway:80/"); // Your container setup
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Match JWT expiry
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Your existing path base configuration
app.UsePathBase("/frontend");
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/frontend")
        context.Response.Redirect("/frontend/");
    else
        await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Add this before MapRazorPages
app.UseAuthorization();
app.MapRazorPages();

app.Run();