using Serilog.Context;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string Header = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(Header, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers[Header] = correlationId;
        }

        LogContext.PushProperty("CorrelationId", correlationId.ToString());
        context.Response.Headers[Header] = correlationId;
        await _next(context);
    }
}