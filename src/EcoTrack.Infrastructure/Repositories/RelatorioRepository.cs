using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Interfaces;
using EcoTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoTrack.Infrastructure.Repositories;

public class RelatorioRepository : GenericRepository<Relatorio>, IRelatorioRepository
{
    public RelatorioRepository(EcoTrackDbContext context) : base(context) { }

    public async Task<(List<Relatorio> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, int? usuarioId)
    {
        var query = _dbSet.Include(r => r.GeradoPor).AsQueryable();

        if (usuarioId.HasValue)
            query = query.Where(r => r.GeradoPorId == usuarioId.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.DataCriacao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
