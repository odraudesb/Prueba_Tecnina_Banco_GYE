using System.ComponentModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Data;

var seedDemo = args.Contains("--seed-demo");
var builder = WebApplication.CreateBuilder(args.Where(arg => arg != "--seed-demo").ToArray());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ShopDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Shop")
    ?? throw new InvalidOperationException("Configura ConnectionStrings:Shop.")));

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

app.MapControllers();
app.Run();
