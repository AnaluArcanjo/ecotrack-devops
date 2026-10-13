using System.Net.Http.Headers;
using System.Net.Http.Json;
using EcoTrack.Application.ViewModels;

namespace EcoTrack.Tests.Factories;

/// <summary>
/// Helper que centraliza o fluxo de login nos testes, evitando duplicação
/// de código para obter um token JWT válido em cada classe de teste.
/// </summary>
public static class TestAuthHelper
{
    public static async Task<string> ObterTokenAsync(HttpClient client)
    {
        var loginRequest = new LoginRequestVM
        {
            Email = "teste@ecotrack.com.br",
            Senha = "Teste@123"
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseVM>();
        return auth!.Token;
    }

    public static async Task<string> ObterTokenAdminAsync(HttpClient client)
    {
        var loginRequest = new LoginRequestVM
        {
            Email = "admin.teste@ecotrack.com.br",
            Senha = "Admin@123"
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseVM>();
        return auth!.Token;
    }

    public static async Task AutenticarComoAdminAsync(HttpClient client)
    {
        var token = await ObterTokenAdminAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static async Task AutenticarAsync(HttpClient client)
    {
        var token = await ObterTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
