var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient("Patients", client =>
{
    client.BaseAddress = new Uri("http://apigateway:80/"); // Fixed: use correct container name and port
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Tell the app it's hosted under /frontend
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
app.UseAuthorization();
app.MapRazorPages();

app.Run();