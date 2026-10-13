using System.Linq;
using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Enums;
using EcoTrack.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcoTrack.Tests.Factories;

/// <summary>
/// Factory customizada que substitui o SQL Server pelo provedor InMemory do EF Core,
/// permitindo executar os testes de integração sem depender de um banco de dados real.
/// Cada instância recebe um banco InMemory com nome único para isolamento entre testes.
/// </summary>
public class EcoTrackWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"EcoTrackTestDb_{Guid.NewGuid()}";

    // Raiz compartilhada: garante que o seed feito abaixo (num ServiceProvider temporário)
    // e a API sob teste enxerguem o MESMO banco InMemory.
    private readonly InMemoryDatabaseRoot _dbRoot = new();

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AutoMigrate"] = "false"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EcoTrackDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            services.AddDbContext<EcoTrackDbContext>(options =>
                options.UseInMemoryDatabase(_dbName, _dbRoot));

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EcoTrackDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    // Observação: o EcoTrackDbContext já cria dados iniciais via HasData (usuários 1-2 e
    // tipos de resíduo 1-6), então as tabelas nunca estão vazias. Por isso cada registro
    // de teste é verificado pelo Id, e não com um "Any()" geral.
    private static void SeedTestData(EcoTrackDbContext db)
    {
        if (!db.Usuarios.Any(u => u.Id == 100))
        {
            db.Usuarios.Add(new Usuario
            {
                Id = 100,
                Nome = "Usuário de Teste",
                Email = "teste@ecotrack.com.br",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Teste@123", workFactor: 4),
                Perfil = PerfilUsuario.Cidadao
            });
        }

        if (!db.Usuarios.Any(u => u.Id == 101))
        {
            db.Usuarios.Add(new Usuario
            {
                Id = 101,
                Nome = "Admin de Teste",
                Email = "admin.teste@ecotrack.com.br",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123", workFactor: 4),
                Perfil = PerfilUsuario.Admin
            });
        }

        if (!db.TiposResiduo.Any(t => t.Id == 100))
        {
            db.TiposResiduo.Add(new TipoResiduo
            {
                Id = 100,
                Nome = "Papel/Papelão Teste",
                Descricao = "Tipo de resíduo para testes",
                CorLixeira = "AZUL",
                Instrucao = "Instrução de teste."
            });
        }

        if (!db.PontosColeta.Any(p => p.Id == 100))
        {
            db.PontosColeta.Add(new PontoColeta
            {
                Id = 100,
                Nome = "Ecoponto de Teste",
                Endereco = "Rua de Teste, 123",
                Cidade = "São Paulo",
                Estado = "SP",
                Cep = "01310-100",
                CapacidadeKg = 500,
                OcupacaoAtual = 100,
                UsuarioId = 100
            });
        }

        db.SaveChanges();
    }
}
