using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Enums;
using EcoTrack.Domain.Interfaces;
using EcoTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoTrack.Infrastructure.Repositories;

public class ColetaRepository : GenericRepository<Coleta>, IColetaRepository
{
    public ColetaRepository(EcoTrackDbContext context) : base(context) { }

    public async Task<(List<Coleta> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, int? usuarioId, StatusColeta? status)
    {
        var query = _dbSet
            .Include(c => c.PontoColeta)
            .Include(c => c.TipoResiduo)
            .Include(c => c.Usuario)
            .AsQueryable();

        if (usuarioId.HasValue)
            query = query.Where(c => c.UsuarioId == usuarioId.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.DataCriacao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(List<Coleta> Items, int TotalCount)> GetPagedByPontoAsync(
        int pontoColetaId, int pageNumber, int pageSize)
    {
        var query = _dbSet
            .Include(c => c.TipoResiduo)
            .Include(c => c.Usuario)
            .Where(c => c.PontoColetaId == pontoColetaId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.DataCriacao)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(long TotalColetas, decimal TotalKg)> GetEstatisticasPeriodoAsync(DateTime inicio, DateTime fim)
    {
        var query = _dbSet.Where(c =>
            c.Status == StatusColeta.Coletada &&
            c.DataColeta.HasValue &&
            c.DataColeta.Value.Date >= inicio.Date &&
            c.DataColeta.Value.Date <= fim.Date);

        var totalColetas = await query.LongCountAsync();
        var totalKg = await query.SumAsync(c => (decimal?)c.QuantidadeKg) ?? 0;

        return (totalColetas, totalKg);
    }

    public async Task<List<(string TipoResiduo, decimal TotalKg)>> GetRankingPorTipoAsync()
    {
        // Agrupa pela FK (TipoResiduoId), não pela navigation property, para manter
        // a query estável tanto em SQL Server quanto no provider InMemory (usado em testes),
        // que historicamente tem comportamento instável ao agrupar por propriedades de navegação.
        var agregadoPorTipo = await _dbSet
            .Where(c => c.Status == StatusColeta.Coletada)
            .GroupBy(c => c.TipoResiduoId)
            .Select(g => new { TipoResiduoId = g.Key, TotalKg = g.Sum(c => c.QuantidadeKg) })
            .ToListAsync();

        if (agregadoPorTipo.Count == 0)
            return new List<(string TipoResiduo, decimal TotalKg)>();

        var tipoIds = agregadoPorTipo.Select(a => a.TipoResiduoId).ToList();
        var nomesPorId = await _context.Set<TipoResiduo>()
            .Where(t => tipoIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.Nome);

        return agregadoPorTipo
            .Select(a => (nomesPorId.GetValueOrDefault(a.TipoResiduoId, "Desconhecido"), a.TotalKg))
            .OrderByDescending(r => r.Item2)
            .ToList();
    }
}
