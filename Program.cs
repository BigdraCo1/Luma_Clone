using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using alma.Contexts;
using alma.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Main") ?? throw new InvalidOperationException("Connection string not found.")));

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddRazorPages().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

var supportedCultures = new[] { "en", "th" };
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

builder.Services.AddHttpClient();

builder.Services.AddScoped<ISessionService, SessionService>();

// Register EmailService
builder.Services.AddScoped<EmailService>();

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
} else {
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

// TODO: Update this to use the accept-language header
app.UseRequestLocalization(localizationOptions);

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers(); // Ensure controllers are mapped

app.Run();