using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Interfaces;
using EcoTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoTrack.Infrastructure.Repositories;

public class AlertaRepository : GenericRepository<Alerta>, IAlertaRepository
{
    public AlertaRepository(EcoTrackDbContext context) : base(context) { }

    public async Task<(List<Alerta> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, bool apenasAbertos)
    {
        var query = _dbSet.Include(a => a.PontoColeta).AsQueryable();

        if (apenasAbertos)
            query = query.Where(a => !a.Resolvido);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.DataCriacao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<Alerta>> GetAbertosByPontoAsync(int pontoColetaId) =>
        await _dbSet.Where(a => a.PontoColetaId == pontoColetaId && !a.Resolvido).ToListAsync();

    public async Task<int> CountAbertosAsync() =>
        await _dbSet.CountAsync(a => !a.Resolvido);
}
