using System.Net.Http.Json;
using EcoTrack.Application.ViewModels;
using EcoTrack.Tests.Factories;
using FluentAssertions;
using Xunit;

namespace EcoTrack.Tests.Controllers;

public class AuthControllerTests : IClassFixture<EcoTrackWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(EcoTrackWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_ReturnsHttpStatusCode200()
    {
        var request = new LoginRequestVM
        {
            Email = "teste@ecotrack.com.br",
            Senha = "Teste@123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<AuthResponseVM>();
        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrEmpty();
        body.Email.Should().Be("teste@ecotrack.com.br");
    }

    [Fact]
    public async Task Login_ComCredenciaisInvalidas_ReturnsHttpStatusCode401()
    {
        var request = new LoginRequestVM { Email = "teste@ecotrack.com.br", Senha = "SenhaErrada123" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Cadastrar_ComDadosValidos_ReturnsHttpStatusCode201()
    {
        var request = new CadastroRequestVM
        {
            Nome = "Novo Usuário Teste",
            Email = $"novo_{Guid.NewGuid():N}@ecotrack.com.br",
            Senha = "Senha@123",
            Perfil = "Cidadao"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/cadastro", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }
}
