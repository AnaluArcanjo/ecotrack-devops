using EcoTrack.Tests.Factories;
using FluentAssertions;
using Xunit;

namespace EcoTrack.Tests.Controllers;

public class NotificacaoControllerTests : IClassFixture<EcoTrackWebApplicationFactory>
{
    private readonly HttpClient _client;

    public NotificacaoControllerTests(EcoTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ListarNotificacoes_ReturnsHttpStatusCode200()
    {
       
        await TestAuthHelper.AutenticarAsync(_client);
        var request = "/api/notificacoes";

       
        var response = await _client.GetAsync(request);

        
        response.EnsureSuccessStatusCode(); 
    }

    [Fact]
    public async Task Get_ContadorNotificacoesNaoLidas_ReturnsHttpStatusCode200()
    {
       
        await TestAuthHelper.AutenticarAsync(_client);
        var request = "/api/notificacoes/contador";

       
        var response = await _client.GetAsync(request);

        
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Get_ListarNotificacoes_SemAutenticacao_ReturnsHttpStatusCode401()
    {
       
        var request = "/api/notificacoes";

       
        var response = await _client.GetAsync(request);

        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
