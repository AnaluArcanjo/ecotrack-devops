using System.Net.Http.Json;
using EcoTrack.Tests.Factories;
using FluentAssertions;
using Xunit;

namespace EcoTrack.Tests.Controllers;

public class PontoColetaControllerTests : IClassFixture<EcoTrackWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PontoColetaControllerTests(EcoTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ListarPontosColeta_ReturnsHttpStatusCode200()
    {
       
        var request = "/api/pontos-coleta";

       
        var response = await _client.GetAsync(request);

        
        response.EnsureSuccessStatusCode(); 
    }

    [Fact]
    public async Task Get_BuscarPontoColetaPorId_ReturnsHttpStatusCode200()
    {
       
        var request = "/api/pontos-coleta/100";

       
        var response = await _client.GetAsync(request);

        
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_PontosColetaCriticos_ReturnsHttpStatusCode200()
    {
       
        var request = "/api/pontos-coleta/criticos";

       
        var response = await _client.GetAsync(request);

        
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_BuscarPontoColetaInexistente_ReturnsHttpStatusCode404()
    {
       
        var request = "/api/pontos-coleta/999999";

       
        var response = await _client.GetAsync(request);

        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Post_CriarPontoColetaSemAutenticacao_ReturnsHttpStatusCode401()
    {
       
        var request = "/api/pontos-coleta";
        var body = new
        {
            nome = "Ecoponto Sem Auth",
            endereco = "Rua Teste, 1",
            cidade = "São Paulo",
            estado = "SP",
            cep = "01000-000",
            capacidadeKg = 100
        };

       
        var response = await _client.PostAsJsonAsync(request, body);

        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
