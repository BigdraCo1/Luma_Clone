using System.Globalization;
using System.Reflection;

using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

using alma.Services;
using alma.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Main") ?? throw new InvalidOperationException("Connection string not found.")));

builder.Services.AddLocalization(options => options.ResourcesPath = "");
builder.Services.AddRazorPages().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options => {
        options.DataAnnotationLocalizerProvider =
            (type, factory) => {
                var assemblyName =
                    new AssemblyName(
                        typeof(DataAnnotationsLocalizer)
                                .GetTypeInfo()
                                .Assembly.FullName!);
                return factory.Create("Services.DataAnnotationsLocalizer", assemblyName.Name!);
            };
    });

var supportedCultures = new[] { "en", "th" };
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// Disable warning about async method not containing any await operators because
// CustomRequestCultureProvider requires an async method but nothing have to be done asynchronously
#pragma warning disable CS1998 
localizationOptions.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context => {
    var userLang = context.Request.Cookies["lang"];

    if (!string.IsNullOrEmpty(userLang) && supportedCultures.Contains(userLang)) {
        var culture = new CultureInfo(userLang);
        return new ProviderCultureResult(culture.Name, culture.Name);
    }

    return new ProviderCultureResult(supportedCultures[0], supportedCultures[0]);
}));

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IIconService, IconService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try {
    var context = services.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
} catch (Exception ex) {

    Console.WriteLine(ex.Message);
}

DatabaseInitilaizer.Seed(app);

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
} else {
    app.UseStatusCodePagesWithRedirects("/error?code={0}");
    app.UseExceptionHandler("/error?code=500");
}

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".avif"] = "image/avif";
app.UseStaticFiles(new StaticFileOptions {
    ContentTypeProvider = provider,
    OnPrepareResponse = ctx => {
        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=31536000, immutable");
    }
});

app.UseRouting();

// TODO: Update this to use the accept-language header
app.UseRequestLocalization(localizationOptions);

app.MapRazorPages();

app.Run();
