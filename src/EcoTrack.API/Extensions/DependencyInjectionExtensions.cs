using System.Text;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.Services;
using EcoTrack.Application.Validators;
using EcoTrack.Domain.Interfaces;
using EcoTrack.Infrastructure.Data;
using EcoTrack.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EcoTrack.API.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddEcoTrackInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EcoTrackDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly("EcoTrack.Infrastructure")));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPontoColetaRepository, PontoColetaRepository>();
        services.AddScoped<ITipoResiduoRepository, TipoResiduoRepository>();
        services.AddScoped<IColetaRepository, ColetaRepository>();
        services.AddScoped<IAlertaRepository, AlertaRepository>();
        services.AddScoped<INotificacaoRepository, NotificacaoRepository>();
        services.AddScoped<IRelatorioRepository, RelatorioRepository>();

        return services;
    }

    public static IServiceCollection AddEcoTrackApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPontoColetaService, PontoColetaService>();
        services.AddScoped<ITipoResiduoService, TipoResiduoService>();
        services.AddScoped<IColetaService, ColetaService>();
        services.AddScoped<IAlertaService, AlertaService>();
        services.AddScoped<INotificacaoService, NotificacaoService>();
        services.AddScoped<IRelatorioService, RelatorioService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        services.AddScoped<EcoTrack.API.Middleware.FluentValidationActionFilter>();

        return services;
    }

    public static IServiceCollection AddEcoTrackAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"]!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ClockSkew = TimeSpan.FromMinutes(2)
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", p => p.RequireRole("Admin"))
            .AddPolicy("AdminOuEmpresa", p => p.RequireRole("Admin", "Empresa"))
            .AddPolicy("AdminOuColetor", p => p.RequireRole("Admin", "Coletor"));

        return services;
    }
}
