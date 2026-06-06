using System.Net;
using System.Text.Json;

namespace WiseWMS.Api.Middleware
{
    /// <summary>
    /// 全局异常中间件
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"发生未处理的异常{ex.Message}");
                await HandelExceptionAsync(context, ex);
            }
        }

        private static Task HandelExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(
                new
                {
                    code = 500,
                    message = ex.Message,
                    data = (object?)null,
                }
            );

            return context.Response.WriteAsync(result);
        }
    }
}
