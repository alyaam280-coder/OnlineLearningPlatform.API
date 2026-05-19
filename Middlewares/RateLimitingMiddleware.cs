using System.Collections.Concurrent;

namespace OnlineLearningPlatform.API.Middleware;


public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    
    private static readonly ConcurrentDictionary<string, List<DateTime>> _requestTimes = new();

    public RateLimitingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
       
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var now = DateTime.UtcNow;

        
        var clientRequests = _requestTimes.GetOrAdd(ipAddress, _ => new List<DateTime>());

        lock (clientRequests)
        {
           
            clientRequests.RemoveAll(time => time < now.AddSeconds(-1));

           
            if (clientRequests.Count >= 5)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "text/plain; charset=utf-8";
                context.Response.WriteAsync("Too many requists");
                return; 
            }

          
            clientRequests.Add(now);
        }

      
        await _next(context);
    }
}
