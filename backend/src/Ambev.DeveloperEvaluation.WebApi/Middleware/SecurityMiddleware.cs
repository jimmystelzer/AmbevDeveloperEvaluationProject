public class SecurityMiddleware
{
    private readonly RequestDelegate _next;
    public SecurityMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var IsDevelopment = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();
        // HSTS (apenas em produção)
        if (!context.Request.IsHttps && !IsDevelopment)
        {
            context.Response.Headers["Strict-Transport-Security"] = "max-age=63072000; includeSubDomains; preload";
        }
        // X-Content-Type-Options
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        // X-Frame-Options
        context.Response.Headers["X-Frame-Options"] = "DENY";
        // X-XSS-Protection (legacy, mas ainda útil para alguns browsers)
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        // Referrer-Policy
        context.Response.Headers["Referrer-Policy"] = "no-referrer";
        // Content-Security-Policy (ajuste conforme necessário para seu frontend)
        context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; base-uri 'self';";
        await _next(context);
    }
}