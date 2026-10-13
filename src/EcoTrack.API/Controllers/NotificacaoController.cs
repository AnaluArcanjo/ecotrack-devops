using System.Security.Claims;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoTrack.API.Controllers;

/// <summary>
/// Gestão de notificações do usuário autenticado.
/// </summary>
[ApiController]
[Route("api/notificacoes")]
[Authorize]
[Produces("application/json")]
public class NotificacaoController : ControllerBase
{
    private readonly INotificacaoService _notificacaoService;

    public NotificacaoController(INotificacaoService notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }

   
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<NotificacaoVM>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<NotificacaoVM>>> Listar(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool apenasNaoLidas = false)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        pageNumber = Math.Max(pageNumber, 1);

        var usuarioId = ObterUsuarioId();
        var result = await _notificacaoService.ListarAsync(usuarioId, pageNumber, pageSize, apenasNaoLidas);
        return Ok(result);
    }

   
    [HttpGet("contador")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> ContarNaoLidas()
    {
        var usuarioId = ObterUsuarioId();
        var total = await _notificacaoService.ContarNaoLidasAsync(usuarioId);
        return Ok(new { naoLidas = total });
    }

   
    [HttpPatch("{id:int}/ler")]
    [ProducesResponseType(typeof(NotificacaoVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificacaoVM>> MarcarComoLida(int id)
    {
        var usuarioId = ObterUsuarioId();
        var result = await _notificacaoService.MarcarComoLidaAsync(id, usuarioId);
        return Ok(result);
    }

    [HttpPatch("ler-todas")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarcarTodasComoLidas()
    {
        var usuarioId = ObterUsuarioId();
        await _notificacaoService.MarcarTodasComoLidasAsync(usuarioId);
        return NoContent();
    }

    private int ObterUsuarioId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
