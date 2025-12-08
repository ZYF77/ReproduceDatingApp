using System.Net;
using System.Text.Json;
using API.Error;

namespace API.Middleware;

public class GlobalExceptionHandlingMiddleware(RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    /// <summary>
    /// 缓存序列化配置，避免每次请求都 new
    /// </summary>
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception : {Message} occurred while processing the request.", ex.Message);
            await HandleExceptionAdync(context,ex) ;
        }
    }

    private async Task HandleExceptionAdync(HttpContext context, Exception ex)
    {

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = environment.IsDevelopment()
            ? new ApiException(context.Response.StatusCode,ex.Message,ex.StackTrace?.ToString())
            : new ApiException(context.Response.StatusCode,"Internal Server Error", null);

        var json = JsonSerializer.Serialize(response,jsonOptions);
        await context.Response.WriteAsync(json);
    }
}
