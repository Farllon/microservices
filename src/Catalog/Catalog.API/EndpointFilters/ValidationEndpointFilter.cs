using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Catalog.API.EndpointFilters;

public class ValidationEndpointFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argToValidate = context.Arguments.FirstOrDefault(
            argument => argument is T);
        
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is null)
            return await next.Invoke(context);

        var validationResult = await validator.ValidateAsync((T)argToValidate!);

        return validationResult.IsValid
            ? await next.Invoke(context)
            : Results.ValidationProblem(
                errors: validationResult.ToDictionary(),
                statusCode: StatusCodes.Status422UnprocessableEntity);
    }
}