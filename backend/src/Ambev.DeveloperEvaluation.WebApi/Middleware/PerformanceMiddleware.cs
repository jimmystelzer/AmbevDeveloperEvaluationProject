using System.Diagnostics;
using Serilog;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    public PerformanceMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        await _next(context);
        sw.Stop();

        Log.Information("Request {Method} {Path} took {Elapsed:0.0000} ms",
            context.Request.Method,
            context.Request.Path,
            sw.Elapsed.TotalMilliseconds);
    }
}