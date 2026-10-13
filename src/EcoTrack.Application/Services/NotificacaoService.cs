using EcoTrack.Application.Exceptions;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Common;
using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Interfaces;

namespace EcoTrack.Application.Services;

public class NotificacaoService : INotificacaoService
{
    private readonly INotificacaoRepository _notificacaoRepository;

    public NotificacaoService(INotificacaoRepository notificacaoRepository)
    {
        _notificacaoRepository = notificacaoRepository;
    }

    public async Task<PagedResult<NotificacaoVM>> ListarAsync(
        int usuarioId, int pageNumber, int pageSize, bool apenasNaoLidas)
    {
        var (items, totalCount) = await _notificacaoRepository
            .GetPagedByUsuarioAsync(usuarioId, pageNumber, pageSize, apenasNaoLidas);

        return new PagedResult<NotificacaoVM>
        {
            Items = items.Select(MapToVM).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<NotificacaoVM> MarcarComoLidaAsync(int id, int usuarioId)
    {
        var notificacao = await _notificacaoRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Notificação", id);

        if (notificacao.UsuarioId != usuarioId)
            throw new UnauthorizedAppException("Você não tem permissão para esta notificação.");

        if (!notificacao.Lida)
        {
            notificacao.Lida = true;
            notificacao.DataLeitura = DateTime.UtcNow;
            await _notificacaoRepository.UpdateAsync(notificacao);
        }

        return MapToVM(notificacao);
    }

    public async Task MarcarTodasComoLidasAsync(int usuarioId) =>
        await _notificacaoRepository.MarcarTodasComoLidasAsync(usuarioId);

    public async Task<int> ContarNaoLidasAsync(int usuarioId) =>
        await _notificacaoRepository.CountNaoLidasAsync(usuarioId);

    private static NotificacaoVM MapToVM(Notificacao n) => new()
    {
        Id = n.Id,
        Titulo = n.Titulo,
        Mensagem = n.Mensagem,
        Tipo = n.Tipo.ToString(),
        Lida = n.Lida,
        DataCriacao = n.DataCriacao,
        DataLeitura = n.DataLeitura
    };
}
