using System.Security.Claims;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoTrack.API.Controllers;

/// <summary>
/// Gestão de coletas de resíduos recicláveis (agendamento, atualização e cancelamento).
/// </summary>
[ApiController]
[Route("api/coletas")]
[Authorize]
[Produces("application/json")]
public class ColetaController : ControllerBase
{
    private readonly IColetaService _coletaService;

    public ColetaController(IColetaService coletaService)
    {
        _coletaService = coletaService;
    }


    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ColetaVM>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ColetaVM>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        pageNumber = Math.Max(pageNumber, 1);

        var isAdmin = User.IsInRole("Admin");
        int? usuarioId = isAdmin ? null : ObterUsuarioId();

        var result = await _coletaService.ListarAsync(pageNumber, pageSize, usuarioId, status);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ColetaVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ColetaVM>> BuscarPorId(int id)
    {
        var result = await _coletaService.BuscarPorIdAsync(id);
        return Ok(result);
    }

    [HttpGet("ponto/{pontoColetaId:int}")]
    [ProducesResponseType(typeof(PagedResult<ColetaVM>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<ColetaVM>>> ListarPorPonto(
        int pontoColetaId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        pageNumber = Math.Max(pageNumber, 1);

        var result = await _coletaService.ListarPorPontoAsync(pontoColetaId, pageNumber, pageSize);
        return Ok(result);
    }

  
    [HttpPost]
    [ProducesResponseType(typeof(ColetaVM), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ColetaVM>> Agendar([FromBody] AgendarColetaVM request)
    {
        var usuarioId = ObterUsuarioId();
        var result = await _coletaService.AgendarAsync(request, usuarioId);
        return CreatedAtAction(nameof(BuscarPorId), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ColetaVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ColetaVM>> Atualizar(int id, [FromBody] AtualizarColetaVM request)
    {
        var result = await _coletaService.AtualizarAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Cancelar(int id)
    {
        await _coletaService.CancelarAsync(id);
        return NoContent();
    }

    private int ObterUsuarioId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
