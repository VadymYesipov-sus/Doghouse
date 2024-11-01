namespace Doghouse.Helpers
{
    public class RateLimiter
    {
        private readonly RequestDelegate _next;
        private readonly int _requestsPerSecond;
        private static Dictionary<string, (DateTime Timestamp, int Count)> _requestCounts = new();

        public RateLimiter(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _requestsPerSecond = configuration.GetValue<int>("RateLimiting:RequestsPerSecond");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string clientIp = context.Connection.RemoteIpAddress.ToString();

            var now = DateTime.UtcNow;
            if (_requestCounts.TryGetValue(clientIp, out var entry))
            {
                if ((now - entry.Timestamp).TotalSeconds < 1)
                {
                    if (entry.Count >= _requestsPerSecond)
                    {
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        await context.Response.WriteAsync("Too many requests. Please try again later.");
                        return;
                    }

                    _requestCounts[clientIp] = (entry.Timestamp, entry.Count + 1);
                }
                else
                {
                    _requestCounts[clientIp] = (now, 1);
                }
            }
            else
            {
                _requestCounts[clientIp] = (now, 1);
            }

            await _next(context);
        }
    }
}
