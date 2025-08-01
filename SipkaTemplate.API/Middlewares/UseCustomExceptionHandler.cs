using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.SecurityTokenService;
using SipkaTemplate.Service.Exceptions;

namespace Day.API.Middlewares
{
    public static class UseCustomExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.ContentType = "application/json";

                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = exceptionFeature?.Error;

                    int statusCode = 500;
                    string message = "Sunucu tarafında beklenmeyen bir hata oluştu.";

                    if (exception is ClientSideException)
                    {
                        statusCode = 400;
                        message = exception.Message;
                    }
                    else if (exception is NotFoundException)
                    {
                        statusCode = 404;
                        message = exception.Message;
                    }
                    else if(exception is BadRequestException)
                    {
                        statusCode = 400;
                        message = exception.Message;
                    }
                    context.Response.StatusCode = statusCode;

                    var jsonResponse = new
                    {
                        statusCode = statusCode,
                        errors = new[] { message }
                    };

                    var json = JsonSerializer.Serialize(jsonResponse);

                    await context.Response.WriteAsync(json);
                });
            });
        }
    }
}
