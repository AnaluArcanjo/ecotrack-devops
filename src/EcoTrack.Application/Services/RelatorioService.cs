using EcoTrack.Application.Exceptions;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Common;
using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Interfaces;

namespace EcoTrack.Application.Services;

public class RelatorioService : IRelatorioService
{
    // Fator estimado: 1kg de resíduo reciclado evita ~2.5kg de emissão de CO2
    private const decimal FatorCo2 = 2.5m;

    private readonly IRelatorioRepository _relatorioRepository;
    private readonly IColetaRepository _coletaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public RelatorioService(
        IRelatorioRepository relatorioRepository,
        IColetaRepository coletaRepository,
        IUsuarioRepository usuarioRepository)
    {
        _relatorioRepository = relatorioRepository;
        _coletaRepository = coletaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<RelatorioVM> GerarAsync(GerarRelatorioVM request, int usuarioId)
    {
        if (request.PeriodoFim < request.PeriodoInicio)
            throw new BusinessRuleException("Data fim deve ser posterior à data início.");

        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
            ?? throw new NotFoundException("Usuário", usuarioId);

        var (totalColetas, totalKg) = await _coletaRepository
            .GetEstatisticasPeriodoAsync(request.PeriodoInicio, request.PeriodoFim);

        var relatorio = new Relatorio
        {
            Titulo = request.Titulo,
            Descricao = request.Descricao,
            PeriodoInicio = request.PeriodoInicio,
            PeriodoFim = request.PeriodoFim,
            TotalColetas = totalColetas,
            TotalKg = totalKg,
            Co2EvitadoKg = totalKg * FatorCo2,
            GeradoPorId = usuarioId
        };

        await _relatorioRepository.AddAsync(relatorio);
        relatorio.GeradoPor = usuario;

        return MapToVM(relatorio);
    }

    public async Task<RelatorioVM> BuscarPorIdAsync(int id)
    {
        var relatorio = await _relatorioRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Relatório", id);
        return MapToVM(relatorio);
    }

    public async Task<PagedResult<RelatorioVM>> ListarAsync(int pageNumber, int pageSize, int? usuarioId)
    {
        var (items, totalCount) = await _relatorioRepository.GetPagedAsync(pageNumber, pageSize, usuarioId);

        return new PagedResult<RelatorioVM>
        {
            Items = items.Select(MapToVM).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    private static RelatorioVM MapToVM(Relatorio r) => new()
    {
        Id = r.Id,
        Titulo = r.Titulo,
        Descricao = r.Descricao,
        PeriodoInicio = r.PeriodoInicio,
        PeriodoFim = r.PeriodoFim,
        TotalKg = r.TotalKg,
        TotalColetas = r.TotalColetas,
        Co2EvitadoKg = r.Co2EvitadoKg,
        GeradoPorNome = r.GeradoPor?.Nome,
        DataCriacao = r.DataCriacao
    };
}
