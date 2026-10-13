using EcoTrack.Tests.Factories;
using FluentAssertions;
using Xunit;

namespace EcoTrack.Tests.Controllers;

public class AlertaControllerTests : IClassFixture<EcoTrackWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AlertaControllerTests(EcoTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ListarAlertas_ComoAdmin_ReturnsHttpStatusCode200()
    {
        await TestAuthHelper.AutenticarComoAdminAsync(_client);
        var request = "/api/alertas";

        var response = await _client.GetAsync(request);

        response.EnsureSuccessStatusCode(); 
    }

    [Fact]
    public async Task Get_ListarAlertas_SemPermissaoAdequada_ReturnsHttpStatusCode403()
    {
        await TestAuthHelper.AutenticarAsync(_client);
        var request = "/api/alertas";

        var response = await _client.GetAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Get_ListarAlertas_SemAutenticacao_ReturnsHttpStatusCode401()
    {
        var request = "/api/alertas";

        var response = await _client.GetAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
