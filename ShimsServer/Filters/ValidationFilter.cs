using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ShimsServer.Filters
{
    public class ValidationFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var modelState = context.HttpContext.Items["ModelState"] as ModelStateDictionary;
            
            // Check if there's a validation result in the context
            if (context.Arguments.FirstOrDefault(x => x?.GetType().Name == "ModelStateDictionary") is ModelStateDictionary ms && !ms.IsValid)
            {
                var errors = ms.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return Results.BadRequest(new { errors });
            }

            return await next(context);
        }
    }

    /// <summary>
    /// Extension method to add validation filter to endpoint groups
    /// </summary>
    public static class ValidationFilterExtensions
    {
        public static RouteGroupBuilder WithValidation(this RouteGroupBuilder group)
        {
            return group.AddEndpointFilter<ValidationFilter>();
        }
    }
}
