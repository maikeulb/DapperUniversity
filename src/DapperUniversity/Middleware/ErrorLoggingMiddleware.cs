using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DapperUniversity.Middlewares
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorLoggingMiddleware> _logger;

        public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
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
                _logger.LogError("Unhandled Exception: {0}", ex);
                throw;
            }
        }
    }
}
