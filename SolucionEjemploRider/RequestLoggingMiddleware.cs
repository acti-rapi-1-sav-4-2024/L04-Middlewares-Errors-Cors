namespace SolucionEjemploRider;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        logger.LogInformation($"Este es el request: {context.Request.Method} {context.Request.Path}");

        await next(context);

        timer.Stop();
        logger.LogInformation($"Finalizó el request. Tiempo que tomó: {timer.ElapsedMilliseconds}ms.");
    }
}