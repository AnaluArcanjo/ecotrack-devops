using EcoTrack.Application.Exceptions;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Common;
using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Enums;
using EcoTrack.Domain.Interfaces;

namespace EcoTrack.Application.Services;

public class ColetaService : IColetaService
{
    private readonly IColetaRepository _coletaRepository;
    private readonly IPontoColetaRepository _pontoColetaRepository;
    private readonly ITipoResiduoRepository _tipoResiduoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly INotificacaoRepository _notificacaoRepository;

    public ColetaService(
        IColetaRepository coletaRepository,
        IPontoColetaRepository pontoColetaRepository,
        ITipoResiduoRepository tipoResiduoRepository,
        IUsuarioRepository usuarioRepository,
        INotificacaoRepository notificacaoRepository)
    {
        _coletaRepository = coletaRepository;
        _pontoColetaRepository = pontoColetaRepository;
        _tipoResiduoRepository = tipoResiduoRepository;
        _usuarioRepository = usuarioRepository;
        _notificacaoRepository = notificacaoRepository;
    }

    public async Task<PagedResult<ColetaVM>> ListarAsync(
        int pageNumber, int pageSize, int? usuarioId, string? status)
    {
        StatusColeta? statusEnum = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<StatusColeta>(status, true, out var parsed))
                throw new BusinessRuleException($"Status '{status}' inválido.");
            statusEnum = parsed;
        }

        var (items, totalCount) = await _coletaRepository.GetPagedAsync(pageNumber, pageSize, usuarioId, statusEnum);

        return new PagedResult<ColetaVM>
        {
            Items = items.Select(MapToVM).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ColetaVM> BuscarPorIdAsync(int id)
    {
        var coleta = await _coletaRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Coleta", id);
        return MapToVM(coleta);
    }

    public async Task<PagedResult<ColetaVM>> ListarPorPontoAsync(int pontoColetaId, int pageNumber, int pageSize)
    {
        if (await _pontoColetaRepository.GetByIdAsync(pontoColetaId) is null)
            throw new NotFoundException("Ponto de coleta", pontoColetaId);

        var (items, totalCount) = await _coletaRepository.GetPagedByPontoAsync(pontoColetaId, pageNumber, pageSize);

        return new PagedResult<ColetaVM>
        {
            Items = items.Select(MapToVM).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ColetaVM> AgendarAsync(AgendarColetaVM request, int usuarioId)
    {
        var ponto = await _pontoColetaRepository.GetByIdAsync(request.PontoColetaId)
            ?? throw new NotFoundException("Ponto de coleta", request.PontoColetaId);

        if (!ponto.Ativo)
            throw new BusinessRuleException("Ponto de coleta inativo. Selecione outro ponto.");

        var tipo = await _tipoResiduoRepository.GetByIdAsync(request.TipoResiduoId)
            ?? throw new NotFoundException("Tipo de resíduo", request.TipoResiduoId);

        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
            ?? throw new NotFoundException("Usuário", usuarioId);

        var coleta = new Coleta
        {
            PontoColetaId = request.PontoColetaId,
            TipoResiduoId = request.TipoResiduoId,
            UsuarioId = usuarioId,
            QuantidadeKg = request.QuantidadeKg,
            Status = request.DataAgendamento.HasValue ? StatusColeta.Agendada : StatusColeta.Pendente,
            DataAgendamento = request.DataAgendamento,
            Observacao = request.Observacao
        };

        await _coletaRepository.AddAsync(coleta);

        var notificacao = new Notificacao
        {
            UsuarioId = usuarioId,
            Titulo = "Coleta registrada com sucesso!",
            Mensagem = $"Sua coleta de {request.QuantidadeKg:F2} kg de {tipo.Nome} no ponto '{ponto.Nome}' foi registrada.",
            Tipo = TipoNotificacao.Sucesso
        };
        await _notificacaoRepository.AddAsync(notificacao);

        coleta.PontoColeta = ponto;
        coleta.TipoResiduo = tipo;
        coleta.Usuario = usuario;

        return MapToVM(coleta);
    }

    public async Task<ColetaVM> AtualizarAsync(int id, AtualizarColetaVM request)
    {
        var coleta = await _coletaRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Coleta", id);

        if (coleta.Status == StatusColeta.Cancelada)
            throw new BusinessRuleException("Coleta cancelada não pode ser alterada.");

        if (coleta.Status == StatusColeta.Coletada)
            throw new BusinessRuleException("Coleta já realizada não pode ser alterada.");

        if (request.Observacao is not null) coleta.Observacao = request.Observacao;
        if (request.DataAgendamento.HasValue) coleta.DataAgendamento = request.DataAgendamento;

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var novoStatus = Enum.Parse<StatusColeta>(request.Status, true);
            coleta.Status = novoStatus;
            if (novoStatus == StatusColeta.Coletada)
                coleta.DataColeta = request.DataColeta ?? DateTime.UtcNow;
        }

        await _coletaRepository.UpdateAsync(coleta);
        return MapToVM(coleta);
    }

    public async Task CancelarAsync(int id)
    {
        var coleta = await _coletaRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Coleta", id);

        if (coleta.Status == StatusColeta.Coletada)
            throw new BusinessRuleException("Coleta já realizada não pode ser cancelada.");

        if (coleta.Status == StatusColeta.Cancelada)
            throw new BusinessRuleException("Coleta já está cancelada.");

        coleta.Status = StatusColeta.Cancelada;
        await _coletaRepository.UpdateAsync(coleta);
    }

    private static ColetaVM MapToVM(Coleta c) => new()
    {
        Id = c.Id,
        PontoColetaId = c.PontoColetaId,
        PontoColetaNome = c.PontoColeta?.Nome,
        TipoResiduoId = c.TipoResiduoId,
        TipoResiduoNome = c.TipoResiduo?.Nome,
        TipoResiduoCor = c.TipoResiduo?.CorLixeira,
        UsuarioId = c.UsuarioId,
        UsuarioNome = c.Usuario?.Nome,
        QuantidadeKg = c.QuantidadeKg,
        Status = c.Status.ToString(),
        DataAgendamento = c.DataAgendamento,
        DataColeta = c.DataColeta,
        Observacao = c.Observacao,
        DataCriacao = c.DataCriacao
    };
}
