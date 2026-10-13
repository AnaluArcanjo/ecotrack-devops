using System.Net;
using System.Text.Json;
using EcoTrack.Tests.Factories;
using FluentAssertions;
using Xunit;

namespace EcoTrack.Tests.Controllers;

/// <summary>
/// Testa o endpoint /health, que é usado pelo HEALTHCHECK do Docker, pelo
/// docker-compose e pelos smoke tests executados após cada deploy no pipeline.
/// </summary>
public class HealthCheckTests : IClassFixture<EcoTrackWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(EcoTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Health_ReturnsHttpStatusCode200()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_Health_RetornaStatusEAmbiente()
    {
        var response = await _client.GetAsync("/health");
        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("status").GetString().Should().Be("Healthy");
        doc.RootElement.GetProperty("environment").GetString().Should().Be("Testing");
    }
}
