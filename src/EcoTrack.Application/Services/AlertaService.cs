using EcoTrack.Application.Exceptions;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Common;
using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Interfaces;

namespace EcoTrack.Application.Services;

public class AlertaService : IAlertaService
{
    private readonly IAlertaRepository _alertaRepository;

    public AlertaService(IAlertaRepository alertaRepository)
    {
        _alertaRepository = alertaRepository;
    }

    public async Task<PagedResult<AlertaVM>> ListarAsync(int pageNumber, int pageSize, bool apenasAbertos)
    {
        var (items, totalCount) = await _alertaRepository.GetPagedAsync(pageNumber, pageSize, apenasAbertos);

        return new PagedResult<AlertaVM>
        {
            Items = items.Select(MapToVM).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<AlertaVM> ResolverAsync(int id)
    {
        var alerta = await _alertaRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Alerta", id);

        if (!alerta.Resolvido)
        {
            alerta.Resolvido = true;
            alerta.DataResolucao = DateTime.UtcNow;
            await _alertaRepository.UpdateAsync(alerta);
        }

        return MapToVM(alerta);
    }

    public async Task<int> ContarAbertosAsync() => await _alertaRepository.CountAbertosAsync();

    private static AlertaVM MapToVM(Alerta a) => new()
    {
        Id = a.Id,
        PontoColetaId = a.PontoColetaId,
        PontoColetaNome = a.PontoColeta?.Nome,
        Tipo = a.Tipo.ToString(),
        Mensagem = a.Mensagem,
        Resolvido = a.Resolvido,
        DataCriacao = a.DataCriacao,
        DataResolucao = a.DataResolucao
    };
}
