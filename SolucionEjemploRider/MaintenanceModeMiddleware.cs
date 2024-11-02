namespace SolucionEjemploRider;

public class MaintenanceModeMiddleware(RequestDelegate next, bool isInMaintenanceMode)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (isInMaintenanceMode && context.Request.Path != "/Maintenance")
        {
            context.Response.Redirect("/Maintenance");
        }
        else
        {
            await next(context);
        }
    }
}