using System.Security.Claims;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoTrack.API.Controllers;

/// <summary>
/// Gestão de pontos de coleta seletiva (ecopontos).
/// </summary>
[ApiController]
[Route("api/pontos-coleta")]
[Produces("application/json")]
public class PontoColetaController : ControllerBase
{
    private readonly IPontoColetaService _pontoColetaService;

    public PontoColetaController(IPontoColetaService pontoColetaService)
    {
        _pontoColetaService = pontoColetaService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<PontoColetaVM>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<PontoColetaVM>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? cidade = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        pageNumber = Math.Max(pageNumber, 1);

        var result = await _pontoColetaService.ListarAsync(pageNumber, pageSize, cidade);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PontoColetaVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PontoColetaVM>> BuscarPorId(int id)
    {
        var result = await _pontoColetaService.BuscarPorIdAsync(id);
        return Ok(result);
    }

    [HttpGet("criticos")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<PontoColetaVM>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PontoColetaVM>>> ListarCriticos()
    {
        var result = await _pontoColetaService.ListarCriticosAsync();
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(PontoColetaVM), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PontoColetaVM>> Criar([FromBody] CriarPontoColetaVM request)
    {
        var usuarioId = ObterUsuarioId();
        var result = await _pontoColetaService.CriarAsync(request, usuarioId);
        return CreatedAtAction(nameof(BuscarPorId), new { id = result.Id }, result);
    }

    [HttpPatch("{id:int}/ocupacao")]
    [Authorize]
    [ProducesResponseType(typeof(PontoColetaVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PontoColetaVM>> AtualizarOcupacao(
        int id, [FromBody] AtualizarOcupacaoVM request)
    {
        var result = await _pontoColetaService.AtualizarOcupacaoAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(int id)
    {
        await _pontoColetaService.DesativarAsync(id);
        return NoContent();
    }

    private int ObterUsuarioId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
