using EcoTrack.Domain.Common;
using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Enums;

namespace EcoTrack.Domain.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveChangesAsync();
}

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
}

public interface IPontoColetaRepository : IGenericRepository<PontoColeta>
{
    Task<(List<PontoColeta> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? cidade);
    Task<List<PontoColeta>> GetCriticosAsync();
}

public interface ITipoResiduoRepository : IGenericRepository<TipoResiduo>
{
    Task<List<TipoResiduo>> GetAtivosAsync();
    Task<bool> ExistsByNomeAsync(string nome);
}

public interface IColetaRepository : IGenericRepository<Coleta>
{
    Task<(List<Coleta> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, int? usuarioId, StatusColeta? status);
    Task<(List<Coleta> Items, int TotalCount)> GetPagedByPontoAsync(int pontoColetaId, int pageNumber, int pageSize);
    Task<(long TotalColetas, decimal TotalKg)> GetEstatisticasPeriodoAsync(DateTime inicio, DateTime fim);
    Task<List<(string TipoResiduo, decimal TotalKg)>> GetRankingPorTipoAsync();
}

public interface IAlertaRepository : IGenericRepository<Alerta>
{
    Task<(List<Alerta> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, bool apenasAbertos);
    Task<List<Alerta>> GetAbertosByPontoAsync(int pontoColetaId);
    Task<int> CountAbertosAsync();
}

public interface INotificacaoRepository : IGenericRepository<Notificacao>
{
    Task<(List<Notificacao> Items, int TotalCount)> GetPagedByUsuarioAsync(
        int usuarioId, int pageNumber, int pageSize, bool apenasNaoLidas);
    Task<int> CountNaoLidasAsync(int usuarioId);
    Task MarcarTodasComoLidasAsync(int usuarioId);
}

public interface IRelatorioRepository : IGenericRepository<Relatorio>
{
    Task<(List<Relatorio> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, int? usuarioId);
}
