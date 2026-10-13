using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Interfaces;

namespace EcoTrack.Application.Services;

public class DashboardService : IDashboardService
{
    private const decimal FatorCo2 = 2.5m;

    private readonly IPontoColetaRepository _pontoColetaRepository;
    private readonly IColetaRepository _coletaRepository;
    private readonly IAlertaRepository _alertaRepository;
    private readonly INotificacaoRepository _notificacaoRepository;

    public DashboardService(
        IPontoColetaRepository pontoColetaRepository,
        IColetaRepository coletaRepository,
        IAlertaRepository alertaRepository,
        INotificacaoRepository notificacaoRepository)
    {
        _pontoColetaRepository = pontoColetaRepository;
        _coletaRepository = coletaRepository;
        _alertaRepository = alertaRepository;
        _notificacaoRepository = notificacaoRepository;
    }

    public async Task<DashboardVM> ObterDashboardAsync(int usuarioId)
    {
        var (_, totalPontos) = await _pontoColetaRepository.GetPagedAsync(1, 1, null);

        var (totalColetas, totalKg) = await _coletaRepository
            .GetEstatisticasPeriodoAsync(new DateTime(2000, 1, 1), DateTime.UtcNow);

        var alertasAbertos = await _alertaRepository.CountAbertosAsync();
        var notifNaoLidas = await _notificacaoRepository.CountNaoLidasAsync(usuarioId);

        var rankingRaw = await _coletaRepository.GetRankingPorTipoAsync();
        var ranking = rankingRaw
            .Select(r => new RankingResiduoVM { TipoResiduo = r.TipoResiduo, TotalKg = r.TotalKg })
            .ToList();

        var pontosCriticos = await _pontoColetaRepository.GetCriticosAsync();
        var pontosCriticosVM = pontosCriticos.Select(p => new PontoColetaVM
        {
            Id = p.Id,
            Nome = p.Nome,
            Endereco = p.Endereco,
            Cidade = p.Cidade,
            Estado = p.Estado,
            Cep = p.Cep,
            Latitude = p.Latitude,
            Longitude = p.Longitude,
            CapacidadeKg = p.CapacidadeKg,
            OcupacaoAtual = p.OcupacaoAtual,
            PercentualOcupacao = p.PercentualOcupacao,
            CapacidadeCritica = p.CapacidadeCritica,
            Ativo = p.Ativo,
            UsuarioId = p.UsuarioId,
            DataCriacao = p.DataCriacao
        }).ToList();

        return new DashboardVM
        {
            TotalPontosColeta = totalPontos,
            TotalColetasRealizadas = totalColetas,
            TotalResiduosKg = totalKg,
            Co2EvitadoTotalKg = totalKg * FatorCo2,
            AlertasAbertos = alertasAbertos,
            NotificacoesNaoLidas = notifNaoLidas,
            RankingResiduos = ranking,
            PontosCriticos = pontosCriticosVM
        };
    }
}
