namespace api.Middleware;

public class ProtocolWhitelistMiddleware
{
    private readonly RequestDelegate _next;

    public ProtocolWhitelistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var url = context.Request.Headers["Referer"].ToString();

        if (!IsProtocolAllowed(url))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Not allowed protocol");
            return;
        }

        await _next(context);
    }

    private static bool IsProtocolAllowed(string url)
    {
        var allowedProtocols = new[] { "http", "https" };

        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               allowedProtocols.Contains(uri.Scheme.ToLower());
    }
}