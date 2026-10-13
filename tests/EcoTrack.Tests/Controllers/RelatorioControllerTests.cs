using System.Net.Http.Json;
using EcoTrack.Application.ViewModels;
using EcoTrack.Tests.Factories;
using FluentAssertions;
using Xunit;

namespace EcoTrack.Tests.Controllers;

public class RelatorioControllerTests : IClassFixture<EcoTrackWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RelatorioControllerTests(EcoTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ListarRelatorios_ComoAdmin_ReturnsHttpStatusCode200()
    {
       
        await TestAuthHelper.AutenticarComoAdminAsync(_client);
        var request = "/api/relatorios";

       
        var response = await _client.GetAsync(request);

        
        response.EnsureSuccessStatusCode(); 
    }

    [Fact]
    public async Task Post_GerarRelatorio_ComoAdmin_ReturnsHttpStatusCode201()
    {
       
        await TestAuthHelper.AutenticarComoAdminAsync(_client);
        var request = new GerarRelatorioVM
        {
            Titulo = "Relatório de Teste",
            Descricao = "Gerado via teste automatizado",
            PeriodoInicio = DateTime.UtcNow.AddDays(-30),
            PeriodoFim = DateTime.UtcNow
        };

       
        var response = await _client.PostAsJsonAsync("/api/relatorios", request);

        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Get_ListarRelatorios_PerfilSemPermissao_ReturnsHttpStatusCode403()
    {
        // Usuário Cidadao não tem acesso a relatórios (Admin/Empresa apenas)
        await TestAuthHelper.AutenticarAsync(_client);
        var request = "/api/relatorios";

       
        var response = await _client.GetAsync(request);

        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }
}
