namespace ProductsMicroService.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            if (e.InnerException is not null)
            {
                _logger.LogError("{ExceptionType}: {ExceptionMessage}", e.InnerException.GetType().ToString(), e.InnerException.Message);
            }
            else
            {
                _logger.LogError("{ExceptionType}: {ExceptionMessage}", e.GetType().ToString(), e.Message);
            }

            await httpContext.Response.WriteAsJsonAsync(new { Message = e.Message, Type = e.GetType().ToString() });
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}