using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Interfaces;
using EcoTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoTrack.Infrastructure.Repositories;

public class PontoColetaRepository : GenericRepository<PontoColeta>, IPontoColetaRepository
{
    public PontoColetaRepository(EcoTrackDbContext context) : base(context) { }

    public async Task<(List<PontoColeta> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, string? cidade)
    {
        var query = _dbSet.Include(p => p.Usuario)
            .Where(p => p.Ativo);

        if (!string.IsNullOrWhiteSpace(cidade))
            query = query.Where(p => p.Cidade.ToLower() == cidade.ToLower());

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Nome)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<PontoColeta>> GetCriticosAsync()
    {
        return await _dbSet
            .Where(p => p.Ativo && p.CapacidadeKg > 0)
            .Where(p => (p.OcupacaoAtual / p.CapacidadeKg) * 100 >= 80)
            .ToListAsync();
    }
}
