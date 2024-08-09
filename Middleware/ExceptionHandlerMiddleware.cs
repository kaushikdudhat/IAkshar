using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace iAkshar.Middleware
{
    public class ExceptionHandlerMiddleware :IMiddleware
    {
        private readonly ILogger _logger;
      
        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                ProblemDetails problem = new ()
                {
                    Detail = e.Message,
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Server Error",
                    Type = "Server Error"
                };

                string json = JsonSerializer.Serialize(problem);
                context.Response.WriteAsync(json);
            }
        }
    }
}
