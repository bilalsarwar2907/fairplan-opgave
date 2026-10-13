using Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);              // run the rest of the pipeline (controller etc.)
            }
            catch (BusinessException ex)           // our own rule errors
            {
                await WriteProblem(context, ex.StatusCode, ex.Message);
            }
            catch (Exception)                      // anything unexpected
            {
                await WriteProblem(context, 500, "Der opstod en uventet fejl.");
            }
        }

        private static Task WriteProblem(HttpContext context, int status, string detail)
        {
            context.Response.StatusCode = status;
            return context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = status == 500 ? "Serverfejl" : "Ugyldig handling",
                Detail = detail
            });
        }
    }
}