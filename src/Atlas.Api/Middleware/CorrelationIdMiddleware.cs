namespace Atlas.Api.Middleware;

public sealed class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        var cid = context.Request.Headers[HeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(cid))
            cid = Guid.NewGuid().ToString("N");

        context.Items[HeaderName] = cid;
        context.Response.Headers[HeaderName] = cid;

        using (context.RequestServices.GetRequiredService<ILogger<CorrelationIdMiddleware>>()
                   .BeginScope(new Dictionary<string, object> { ["correlationId"] = cid }))
        {
            await _next(context);
        }
    }
}
