using ISHE_Data.Models.Internal;
using ISHE_Utility.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace ISHE_API.Configurations.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            //error response
            var errorResponse = new ErrorModel
            {
                message = exception.Message
            };

            if (exception is ConflictException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            else if (exception is NotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else if (exception is BadRequestException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var result = JsonConvert.SerializeObject(errorResponse);
            return context.Response.WriteAsync(result);
        }
    }
}
