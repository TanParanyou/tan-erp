using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using TanErp.Api.Authentication;
using TanErp.Api.OpenApi;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;
using TanErp.Infrastructure.Common;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Localization configuration: default 'th', supported 'th', 'en'
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(OpenApiConfiguration.ConfigureSwaggerGen);

// Database Context
var connectionString = builder.Configuration.GetConnectionString("Database")
    ?? "Host=localhost;Database=tan_erp;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddSingleton<IClock, TanErp.Infrastructure.Common.SystemClock>();
builder.Services.AddSingleton<IFirebaseTokenVerifier, FirebaseTokenVerifier>();
builder.Services.AddScoped<ICurrentUserReader, CurrentUserReader>();
builder.Services.AddScoped<GetCurrentUserHandler>();

// Authentication & Authorization
builder.Services.AddAuthentication(FirebaseAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(
        FirebaseAuthenticationHandler.SchemeName, null);

builder.Services.AddAuthorization();

var app = builder.Build();

// Localization Middleware
var supportedCultures = new[] { new CultureInfo("th"), new CultureInfo("en") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("th"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
localizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
{
    new AcceptLanguageHeaderRequestCultureProvider()
};
app.UseRequestLocalization(localizationOptions);

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

namespace TanErp.Api
{
    public partial class Program { }
}
