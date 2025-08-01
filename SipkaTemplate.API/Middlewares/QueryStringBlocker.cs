using System.Text.Json;
using SipkaTemplate.Core.DTOs.HelperDTOs;

namespace Day.API.Middlewares
{
    public class QueryStringBlocker
    {
        private readonly RequestDelegate _next;

        public QueryStringBlocker(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.QueryString.HasValue)
            {
                var response = CustomResponseDto<NoContentDto>.Fail(StatusCodes.Status400BadRequest, "Query parameters are not allowed...");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            await _next(context);
        }
    }
}
