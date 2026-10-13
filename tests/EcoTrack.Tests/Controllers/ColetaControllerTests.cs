using System.Net.Http.Json;
using EcoTrack.Application.ViewModels;
using EcoTrack.Tests.Factories;
using FluentAssertions;
using Xunit;

namespace EcoTrack.Tests.Controllers;

public class ColetaControllerTests : IClassFixture<EcoTrackWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ColetaControllerTests(EcoTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ListarColetas_Autenticado_ReturnsHttpStatusCode200()
    {
        await TestAuthHelper.AutenticarAsync(_client);
        var request = "/api/coletas";

        var response = await _client.GetAsync(request);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_ListarColetas_SemAutenticacao_ReturnsHttpStatusCode401()
    {
        using var clientSemAuth = new EcoTrackWebApplicationFactory().CreateClient();
        var request = "/api/coletas";

        var response = await clientSemAuth.GetAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_AgendarColeta_ComDadosValidos_ReturnsHttpStatusCode201()
    {
        await TestAuthHelper.AutenticarAsync(_client);
        var request = new AgendarColetaVM
        {
            PontoColetaId = 100,
            TipoResiduoId = 100,
            QuantidadeKg = 5.5m,
            Observacao = "Coleta de teste"
        };

        var response = await _client.PostAsJsonAsync("/api/coletas", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Get_ListarColetasPorPonto_ReturnsHttpStatusCode200()
    {
        await TestAuthHelper.AutenticarAsync(_client);
        var request = "/api/coletas/ponto/100";

        var response = await _client.GetAsync(request);

        response.EnsureSuccessStatusCode();
    }
}
