using System.ComponentModel;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
