using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Common;

namespace EcoTrack.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseVM> LoginAsync(LoginRequestVM request);
    Task<AuthResponseVM> CadastrarAsync(CadastroRequestVM request);
}

public interface IPontoColetaService
{
    Task<PagedResult<PontoColetaVM>> ListarAsync(int pageNumber, int pageSize, string? cidade);
    Task<PontoColetaVM> BuscarPorIdAsync(int id);
    Task<List<PontoColetaVM>> ListarCriticosAsync();
    Task<PontoColetaVM> CriarAsync(CriarPontoColetaVM request, int usuarioId);
    Task<PontoColetaVM> AtualizarOcupacaoAsync(int id, AtualizarOcupacaoVM request);
    Task DesativarAsync(int id);
}

public interface ITipoResiduoService
{
    Task<List<TipoResiduoVM>> ListarAtivosAsync();
    Task<TipoResiduoVM> BuscarPorIdAsync(int id);
    Task<TipoResiduoVM> CriarAsync(CriarTipoResiduoVM request);
    Task<TipoResiduoVM> AtualizarAsync(int id, CriarTipoResiduoVM request);
    Task DesativarAsync(int id);
}

public interface IColetaService
{
    Task<PagedResult<ColetaVM>> ListarAsync(int pageNumber, int pageSize, int? usuarioId, string? status);
    Task<ColetaVM> BuscarPorIdAsync(int id);
    Task<PagedResult<ColetaVM>> ListarPorPontoAsync(int pontoColetaId, int pageNumber, int pageSize);
    Task<ColetaVM> AgendarAsync(AgendarColetaVM request, int usuarioId);
    Task<ColetaVM> AtualizarAsync(int id, AtualizarColetaVM request);
    Task CancelarAsync(int id);
}

public interface IAlertaService
{
    Task<PagedResult<AlertaVM>> ListarAsync(int pageNumber, int pageSize, bool apenasAbertos);
    Task<AlertaVM> ResolverAsync(int id);
    Task<int> ContarAbertosAsync();
}

public interface INotificacaoService
{
    Task<PagedResult<NotificacaoVM>> ListarAsync(int usuarioId, int pageNumber, int pageSize, bool apenasNaoLidas);
    Task<NotificacaoVM> MarcarComoLidaAsync(int id, int usuarioId);
    Task MarcarTodasComoLidasAsync(int usuarioId);
    Task<int> ContarNaoLidasAsync(int usuarioId);
}

public interface IRelatorioService
{
    Task<RelatorioVM> GerarAsync(GerarRelatorioVM request, int usuarioId);
    Task<RelatorioVM> BuscarPorIdAsync(int id);
    Task<PagedResult<RelatorioVM>> ListarAsync(int pageNumber, int pageSize, int? usuarioId);
}

public interface IDashboardService
{
    Task<DashboardVM> ObterDashboardAsync(int usuarioId);
}

public interface IJwtTokenGenerator
{
    string GerarToken(int usuarioId, string email, string perfil, out DateTime expiraEm);
}
