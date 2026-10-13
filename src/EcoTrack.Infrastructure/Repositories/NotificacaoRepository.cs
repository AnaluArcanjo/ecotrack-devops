using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Interfaces;
using EcoTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoTrack.Infrastructure.Repositories;

public class NotificacaoRepository : GenericRepository<Notificacao>, INotificacaoRepository
{
    public NotificacaoRepository(EcoTrackDbContext context) : base(context) { }

    public async Task<(List<Notificacao> Items, int TotalCount)> GetPagedByUsuarioAsync(
        int usuarioId, int pageNumber, int pageSize, bool apenasNaoLidas)
    {
        var query = _dbSet.Where(n => n.UsuarioId == usuarioId);

        if (apenasNaoLidas)
            query = query.Where(n => !n.Lida);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(n => n.DataCriacao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<int> CountNaoLidasAsync(int usuarioId) =>
        await _dbSet.CountAsync(n => n.UsuarioId == usuarioId && !n.Lida);

    public async Task MarcarTodasComoLidasAsync(int usuarioId)
    {
        var naoLidas = await _dbSet
            .Where(n => n.UsuarioId == usuarioId && !n.Lida)
            .ToListAsync();

        foreach (var n in naoLidas)
        {
            n.Lida = true;
            n.DataLeitura = DateTime.UtcNow;
        }

        await SaveChangesAsync();
    }
}
