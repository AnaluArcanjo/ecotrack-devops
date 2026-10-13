using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Interfaces;
using EcoTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EcoTrack.Infrastructure.Repositories;

public class TipoResiduoRepository : GenericRepository<TipoResiduo>, ITipoResiduoRepository
{
    public TipoResiduoRepository(EcoTrackDbContext context) : base(context) { }

    public async Task<List<TipoResiduo>> GetAtivosAsync() =>
        await _dbSet.Where(t => t.Ativo).OrderBy(t => t.Nome).ToListAsync();

    public async Task<bool> ExistsByNomeAsync(string nome) =>
        await _dbSet.AnyAsync(t => t.Nome.ToLower() == nome.ToLower());
}
