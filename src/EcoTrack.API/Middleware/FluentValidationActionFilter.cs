using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EcoTrack.API.Middleware;

/// <summary>
/// Action filter que executa validações FluentValidation registradas via DI antes da
/// execução das actions. Substitui o pacote FluentValidation.AspNetCore (descontinuado),
/// mantendo a mesma experiência de validação automática nos endpoints.
/// As falhas são lançadas como FluentValidation.ValidationException e tratadas pelo
/// ExceptionHandlingMiddleware, padronizando a resposta de erro 400.
/// </summary>
public class FluentValidationActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            if (validator is not null)
            {
                var validationContext = new ValidationContext<object>(argument);
                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                    throw new ValidationException(result.Errors);
            }
        }

        await next();
    }
}
