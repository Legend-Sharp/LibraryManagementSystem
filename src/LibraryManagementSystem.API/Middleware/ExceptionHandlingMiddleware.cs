using System.Net;
using System.Text.Json;

namespace LibraryManagementSystem.API.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Request failed: {Path}", ctx.Request.Path);
            await WriteProblem(ctx, ex);
        }
    }

    private static async Task WriteProblem(HttpContext ctx, Exception ex)
    {
        var (status, title) = ex switch
        {
            KeyNotFoundException      => (HttpStatusCode.NotFound, "Resource not found"),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid operation"),
            _                         => (HttpStatusCode.InternalServerError, "Server error")
        };

        var problem = new
        {
            type   = "about:blank",
            title,
            status = (int)status,
            detail = ex.Message,
            traceId = ctx.TraceIdentifier
        };

        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = (int)status;
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}