using GrowthNoteV3.Components;
using GrowthNoteV3.Data;
using GrowthNoteV3.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<DeviceDetectionService>();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("接続文字列 'Default' が appsettings.json に設定されていません。");

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));

var app = builder.Build();

app.UsePathBase("/CGNote");

app.Use(async (context, next) =>
{
    if (!context.Request.Path.HasValue || context.Request.Path == "/")
    {
        context.Response.Redirect($"{context.Request.PathBase}/Home", permanent: false);
        return;
    }

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

