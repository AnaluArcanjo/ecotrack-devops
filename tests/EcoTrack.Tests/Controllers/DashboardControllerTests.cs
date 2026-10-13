using EcoTrack.Tests.Factories;
using FluentAssertions;
using Xunit;

namespace EcoTrack.Tests.Controllers;

public class DashboardControllerTests : IClassFixture<EcoTrackWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DashboardControllerTests(EcoTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ObterDashboard_ReturnsHttpStatusCode200()
    {
        
        await TestAuthHelper.AutenticarAsync(_client);
        var request = "/api/dashboard";

        
        var response = await _client.GetAsync(request);

      
        response.EnsureSuccessStatusCode(); 
    }

    [Fact]
    public async Task Get_ObterDashboard_SemAutenticacao_ReturnsHttpStatusCode401()
    {
       
        var request = "/api/dashboard";

       
        var response = await _client.GetAsync(request);

        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}
