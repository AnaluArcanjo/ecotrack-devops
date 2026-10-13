using System.Net;
using System.Text.Json;
using EcoTrack.Application.Exceptions;
using FluentValidation;

namespace EcoTrack.API.Middleware;

/// <summary>
/// Middleware global de tratamento de exceções. Captura erros não tratados nos controllers
/// e os converte em respostas HTTP padronizadas, evitando exposição de stack traces.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, mensagem, erros) = exception switch
        {
            NotFoundException notFound =>
                (HttpStatusCode.NotFound, notFound.Message, (IDictionary<string, string[]>?)null),

            BusinessRuleException business =>
                (HttpStatusCode.UnprocessableEntity, business.Message, null),

            UnauthorizedAppException unauthorized =>
                (HttpStatusCode.Unauthorized, unauthorized.Message, null),

            ValidationException validation =>
                (HttpStatusCode.BadRequest, "Erro de validação. Verifique os campos informados.",
                    (IDictionary<string, string[]>?)validation.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),

            _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor. Tente novamente mais tarde.", null)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Erro não tratado em {Path}", context.Request.Path);
        else
            _logger.LogWarning("{ExceptionType}: {Message}", exception.GetType().Name, exception.Message);

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            timestamp = DateTime.UtcNow,
            status = (int)statusCode,
            erro = statusCode.ToString(),
            mensagem,
            path = context.Request.Path.Value,
            campos = erros
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
