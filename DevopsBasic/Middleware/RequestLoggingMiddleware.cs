using System.Diagnostics;

namespace DevopsBasic.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    { _next = next; _logger = logger; }

    public async Task Invoke(HttpContext ctx)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("HTTP {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
            await _next(ctx);
            sw.Stop();
            _logger.LogInformation("Completed {StatusCode} in {Elapsed} ms",
                ctx.Response.StatusCode, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Unhandled error after {Elapsed} ms", sw.ElapsedMilliseconds);
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await ctx.Response.WriteAsJsonAsync(new { error = "Unexpected error" });
        }
    }
}
