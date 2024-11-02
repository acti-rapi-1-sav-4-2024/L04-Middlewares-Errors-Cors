using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SolucionEjemploRider;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://example.com", "http://www.contoso.com");
        });
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

bool isInMaintenanceMode = false;
    
var app = builder.Build();

//app.UseExceptionHandler("/error");
app.UseCustomExceptionHandlerMiddleware();

/*app.UseExceptionHandler(exceptionhandlerApp =>
{
    exceptionhandlerApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        
        context.Response.ContentType = MediaTypeNames.Text.Plain;
        
        await context.Response.WriteAsync("Ha ocurrido una excepción.");
    });
});*/


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<MaintenanceModeMiddleware>(isInMaintenanceMode);

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/provocar-error", () =>
{
    throw new Exception("Ejemplo de excepción provocada");
});

//app.MapGet("/error", () => "Ocurrió un error inesperado.");

app.MapGet("/weatherforecast", () =>
{
    
    var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapGet("/Maintenance", () => "Estamos en mantenimiento");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}