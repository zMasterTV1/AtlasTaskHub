using System.Net;
using System.Text.Json;

namespace Atlas.Api.Middleware;

public sealed class ProblemDetailsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ProblemDetailsMiddleware> _logger;

    public ProblemDetailsMiddleware(RequestDelegate next, ILogger<ProblemDetailsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error");

            var status = ex is InvalidOperationException ? HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError;

            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/problem+json";

            var payload = new
            {
                type = "about:blank",
                title = status == HttpStatusCode.BadRequest ? "Bad Request" : "Internal Server Error",
                status = (int)status,
                detail = status == HttpStatusCode.BadRequest ? ex.Message : "Unexpected error.",
                correlationId = context.Response.Headers["X-Correlation-Id"].ToString()
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
