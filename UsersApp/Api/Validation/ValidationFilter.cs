using System.Net;
using FluentValidation;

namespace UsersApp.Api.Validation;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext efiContext, 
        EndpointFilterDelegate next)
    {
        var validator = efiContext.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is null) return await next(efiContext);
        
        var entity = efiContext.Arguments.OfType<T>().FirstOrDefault();

        if (entity is null) return await next(efiContext);
        
        var validationResult = await validator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary(),
                statusCode: (int)HttpStatusCode.UnprocessableEntity);
        }

        return await next(efiContext);
    }
}