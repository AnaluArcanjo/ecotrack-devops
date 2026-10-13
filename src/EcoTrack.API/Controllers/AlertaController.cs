using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoTrack.API.Controllers;

/// <summary>
/// Gestão de alertas automáticos gerados pelos pontos de coleta (ex.: capacidade crítica).
/// Acesso restrito a Administradores e Coletores.
/// </summary>
[ApiController]
[Route("api/alertas")]
[Authorize(Policy = "AdminOuColetor")]
[Produces("application/json")]
public class AlertaController : ControllerBase
{
    private readonly IAlertaService _alertaService;

    public AlertaController(IAlertaService alertaService)
    {
        _alertaService = alertaService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AlertaVM>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AlertaVM>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool apenasAbertos = true)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        pageNumber = Math.Max(pageNumber, 1);

        var result = await _alertaService.ListarAsync(pageNumber, pageSize, apenasAbertos);
        return Ok(result);
    }

    [HttpGet("contador")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> ContarAbertos()
    {
        var total = await _alertaService.ContarAbertosAsync();
        return Ok(new { abertos = total });
    }

    [HttpPatch("{id:int}/resolver")]
    [ProducesResponseType(typeof(AlertaVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertaVM>> Resolver(int id)
    {
        var result = await _alertaService.ResolverAsync(id);
        return Ok(result);
    }
}
