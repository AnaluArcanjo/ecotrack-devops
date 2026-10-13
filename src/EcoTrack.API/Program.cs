using System.Linq;
using EcoTrack.API.Extensions;
using EcoTrack.API.Middleware;
using EcoTrack.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationActionFilter>();
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddEcoTrackInfrastructure(builder.Configuration);
builder.Services.AddEcoTrackApplication();
builder.Services.AddEcoTrackAuthentication(builder.Configuration);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EcoTrack API",
        Version = "v1",
        Description = "API RESTful para Gestão de Resíduos e Reciclagem — ESG Tema 2. " +
                      "Desenvolvida em .NET Core 8 seguindo arquitetura MVVM em camadas.",
        Contact = new OpenApiContact { Name = "EcoTrack Team", Email = "dev@ecotrack.com.br" }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT obtido em POST /api/auth/login"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var erros = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

        var response = new
        {
            timestamp = DateTime.UtcNow,
            status = 400,
            erro = "BadRequest",
            mensagem = "Erro de validação. Verifique os campos informados.",
            path = context.HttpContext.Request.Path.Value,
            campos = erros
        };

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("EcoTrackCors", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Configuration.GetValue<bool>("AutoMigrate", true))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<EcoTrackDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Falha ao aplicar migrações automaticamente. Verifique a conexão com o banco.");
    }
}

app.UseExceptionHandling();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoTrack API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("EcoTrackCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapGet("/health", (IConfiguration config, IWebHostEnvironment env) => Results.Ok(new
    {
        status = "Healthy",
        application = "EcoTrack API - Cidades ESG Inteligentes",
        environment = env.EnvironmentName,
        version = config["APP_VERSION"] ?? "local",
        timestamp = DateTime.UtcNow
    }))
    .ExcludeFromDescription();

app.Run();

public partial class Program { }
