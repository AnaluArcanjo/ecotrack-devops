using System.Security.Claims;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoTrack.API.Controllers;

/// <summary>
/// Geração e consulta de relatórios de reciclagem e impacto ambiental (CO2 evitado).
/// Acesso restrito a Administradores e Empresas.
/// </summary>
[ApiController]
[Route("api/relatorios")]
[Authorize(Policy = "AdminOuEmpresa")]
[Produces("application/json")]
public class RelatorioController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatorioController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<RelatorioVM>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<RelatorioVM>>> Listar(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        pageNumber = Math.Max(pageNumber, 1);

        var isAdmin = User.IsInRole("Admin");
        int? usuarioId = isAdmin ? null : ObterUsuarioId();

        var result = await _relatorioService.ListarAsync(pageNumber, pageSize, usuarioId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RelatorioVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RelatorioVM>> BuscarPorId(int id)
    {
        var result = await _relatorioService.BuscarPorIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RelatorioVM), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<RelatorioVM>> Gerar([FromBody] GerarRelatorioVM request)
    {
        var usuarioId = ObterUsuarioId();
        var result = await _relatorioService.GerarAsync(request, usuarioId);
        return CreatedAtAction(nameof(BuscarPorId), new { id = result.Id }, result);
    }

    private int ObterUsuarioId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
