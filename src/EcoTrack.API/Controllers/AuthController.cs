using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EcoTrack.API.Controllers;

/// <summary>
/// Endpoints de autenticação e cadastro de usuários do sistema.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseVM>> Login([FromBody] LoginRequestVM request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result);
    }

    [HttpPost("cadastro")]
    [ProducesResponseType(typeof(AuthResponseVM), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AuthResponseVM>> Cadastrar([FromBody] CadastroRequestVM request)
    {
        var result = await _authService.CadastrarAsync(request);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
