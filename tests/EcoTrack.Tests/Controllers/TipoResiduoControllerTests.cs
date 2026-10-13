using System.Net.Http.Json;
using EcoTrack.Tests.Factories;
using FluentAssertions;
using Xunit;

namespace EcoTrack.Tests.Controllers;

public class TipoResiduoControllerTests : IClassFixture<EcoTrackWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TipoResiduoControllerTests(EcoTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ListarTiposResiduo_ReturnsHttpStatusCode200()
    {
       
        var request = "/api/tipos-residuo";

       
        var response = await _client.GetAsync(request);

       
        response.EnsureSuccessStatusCode(); 
    }

    [Fact]
    public async Task Get_BuscarTipoResiduoPorId_ReturnsHttpStatusCode200()
    {
       
        var request = "/api/tipos-residuo/100";

       
        var response = await _client.GetAsync(request);

       
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Post_CriarTipoResiduoSemAutenticacao_ReturnsHttpStatusCode401()
    {
       
        var request = "/api/tipos-residuo";
        var body = new
        {
            nome = "Têxtil Teste",
            corLixeira = "ROSA",
            instrucao = "Instrução de teste para têxtil."
        };

       
        var response = await _client.PostAsJsonAsync(request, body);

       
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
