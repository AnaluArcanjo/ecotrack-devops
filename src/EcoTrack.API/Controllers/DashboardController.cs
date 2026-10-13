using System.Security.Claims;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoTrack.API.Controllers;

/// <summary>
/// Indicadores gerais de sustentabilidade e reciclagem (dashboard consolidado).
/// </summary>
[ApiController]
[Route("api/dashboard")]
[Authorize]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Retorna indicadores consolidados: total de resíduos coletados, CO2 evitado,
    /// alertas abertos, ranking de tipos de resíduo e pontos com capacidade crítica.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardVM), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardVM>> ObterDashboard()
    {
        var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _dashboardService.ObterDashboardAsync(usuarioId);
        return Ok(result);
    }
}
