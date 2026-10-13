using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoTrack.API.Controllers;

/// <summary>
/// Gestão dos tipos de resíduo e instruções de descarte correto.
/// </summary>
[ApiController]
[Route("api/tipos-residuo")]
[Produces("application/json")]
public class TipoResiduoController : ControllerBase
{
    private readonly ITipoResiduoService _tipoResiduoService;

    public TipoResiduoController(ITipoResiduoService tipoResiduoService)
    {
        _tipoResiduoService = tipoResiduoService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<TipoResiduoVM>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TipoResiduoVM>>> Listar()
    {
        var result = await _tipoResiduoService.ListarAtivosAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TipoResiduoVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TipoResiduoVM>> BuscarPorId(int id)
    {
        var result = await _tipoResiduoService.BuscarPorIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(TipoResiduoVM), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TipoResiduoVM>> Criar([FromBody] CriarTipoResiduoVM request)
    {
        var result = await _tipoResiduoService.CriarAsync(request);
        return CreatedAtAction(nameof(BuscarPorId), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(TipoResiduoVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TipoResiduoVM>> Atualizar(int id, [FromBody] CriarTipoResiduoVM request)
    {
        var result = await _tipoResiduoService.AtualizarAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Desativar(int id)
    {
        await _tipoResiduoService.DesativarAsync(id);
        return NoContent();
    }
}
