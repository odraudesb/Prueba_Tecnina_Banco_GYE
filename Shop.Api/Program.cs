using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shop.Api.Data;
using Shop.Api.Models;
using Shop.Api.OpenApi;
using Shop.Api.Services;

var seedDemo = args.Contains("--seed-demo");
var builder = WebApplication.CreateBuilder(args.Where(arg => arg != "--seed-demo").ToArray());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.OperationFilter<BearerSecurityFilter>();
});
builder.Services.AddDbContext<ShopDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Shop")
    ?? throw new InvalidOperationException("Configura ConnectionStrings:Shop.")));

var configuredKey = builder.Configuration["Jwt:Key"];
var keyBytes = string.IsNullOrWhiteSpace(configuredKey) && builder.Environment.IsDevelopment()
    ? RandomNumberGenerator.GetBytes(32)
    : Encoding.UTF8.GetBytes(configuredKey ?? string.Empty);
if (keyBytes.Length < 32)
    throw new InvalidOperationException("Configura Jwt__Key con una clave aleatoria de al menos 32 bytes.");

var signingKey = new SymmetricSecurityKey(keyBytes);
builder.Services.AddSingleton(signingKey);
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = AuthService.Issuer,
        ValidAudience = AuthService.Audience,
        IssuerSigningKey = signingKey,
        ValidateIssuerSigningKey = true,
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

if (seedDemo)
{
    if (!app.Environment.IsDevelopment())
        throw new InvalidOperationException("Los datos semilla solo se cargan en Development.");

    var password = app.Configuration["Demo:Password"];
    if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
        throw new InvalidOperationException("Configura Demo__Password con al menos 12 caracteres.");

    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    await DevelopmentSeed.RunAsync(db, password);
    app.Logger.LogInformation("Datos de demostración disponibles.");
    return;
}

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

    if (app.Configuration.GetValue<bool>("OpenSwaggerOnStartup"))
    {
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            var address = app.Urls.Select(url => new Uri(url)).FirstOrDefault(url => url.IsLoopback);
            if (address is null)
                return;

            var swaggerUrl = new Uri(address, "/swagger");
            try
            {
                using var browser = Process.Start(new ProcessStartInfo(swaggerUrl.AbsoluteUri)
                {
                    UseShellExecute = true
                });
            }
            catch (Win32Exception exception)
            {
                app.Logger.LogWarning(exception, "No se pudo abrir el navegador. Abre {SwaggerUrl}", swaggerUrl);
            }
        });
    }
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
