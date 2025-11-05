namespace LibraryManagementSystem.API.Middleware;

public sealed class RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
{
    public async Task Invoke(HttpContext ctx)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await next(ctx);
        sw.Stop();
        logger.LogInformation("HTTP {Method} {Path} -> {Status} in {ElapsedMs} ms",
            ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode, sw.Elapsed.TotalMilliseconds);
    }
}