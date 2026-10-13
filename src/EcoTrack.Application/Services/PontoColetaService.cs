using EcoTrack.Application.Exceptions;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Common;
using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Interfaces;

namespace EcoTrack.Application.Services;

public class PontoColetaService : IPontoColetaService
{
    private readonly IPontoColetaRepository _pontoColetaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAlertaRepository _alertaRepository;

    public PontoColetaService(
        IPontoColetaRepository pontoColetaRepository,
        IUsuarioRepository usuarioRepository,
        IAlertaRepository alertaRepository)
    {
        _pontoColetaRepository = pontoColetaRepository;
        _usuarioRepository = usuarioRepository;
        _alertaRepository = alertaRepository;
    }

    public async Task<PagedResult<PontoColetaVM>> ListarAsync(int pageNumber, int pageSize, string? cidade)
    {
        var (items, totalCount) = await _pontoColetaRepository.GetPagedAsync(pageNumber, pageSize, cidade);

        return new PagedResult<PontoColetaVM>
        {
            Items = items.Select(MapToVM).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PontoColetaVM> BuscarPorIdAsync(int id)
    {
        var ponto = await _pontoColetaRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Ponto de coleta", id);
        return MapToVM(ponto);
    }

    public async Task<List<PontoColetaVM>> ListarCriticosAsync()
    {
        var pontos = await _pontoColetaRepository.GetCriticosAsync();
        return pontos.Select(MapToVM).ToList();
    }

    public async Task<PontoColetaVM> CriarAsync(CriarPontoColetaVM request, int usuarioId)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
            ?? throw new NotFoundException("Usuário", usuarioId);

        var ponto = new PontoColeta
        {
            Nome = request.Nome,
            Endereco = request.Endereco,
            Cidade = request.Cidade,
            Estado = request.Estado.ToUpper(),
            Cep = request.Cep,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CapacidadeKg = request.CapacidadeKg,
            OcupacaoAtual = 0,
            UsuarioId = usuarioId
        };

        await _pontoColetaRepository.AddAsync(ponto);
        ponto.Usuario = usuario;

        return MapToVM(ponto);
    }

    public async Task<PontoColetaVM> AtualizarOcupacaoAsync(int id, AtualizarOcupacaoVM request)
    {
        var ponto = await _pontoColetaRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Ponto de coleta", id);

        if (request.OcupacaoAtual > ponto.CapacidadeKg)
            throw new BusinessRuleException(
                $"Ocupação não pode exceder a capacidade de {ponto.CapacidadeKg} kg.");

        ponto.OcupacaoAtual = request.OcupacaoAtual;
        await _pontoColetaRepository.UpdateAsync(ponto);

        // Geração automática de alerta quando capacidade crítica (>= 80%)
        if (ponto.CapacidadeCritica)
        {
            var alertasAbertos = await _alertaRepository.GetAbertosByPontoAsync(id);
            if (alertasAbertos.Count == 0)
            {
                var alerta = new Alerta
                {
                    PontoColetaId = id,
                    Tipo = Domain.Enums.TipoAlerta.Capacidade,
                    Mensagem = $"Ponto de coleta '{ponto.Nome}' atingiu {ponto.PercentualOcupacao:F1}% " +
                               $"da capacidade ({ponto.OcupacaoAtual}/{ponto.CapacidadeKg} kg). Coleta urgente necessária.",
                    Resolvido = false
                };
                await _alertaRepository.AddAsync(alerta);
            }
        }

        return MapToVM(ponto);
    }

    public async Task DesativarAsync(int id)
    {
        var ponto = await _pontoColetaRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Ponto de coleta", id);
        ponto.Ativo = false;
        await _pontoColetaRepository.UpdateAsync(ponto);
    }

    private static PontoColetaVM MapToVM(PontoColeta p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Endereco = p.Endereco,
        Cidade = p.Cidade,
        Estado = p.Estado,
        Cep = p.Cep,
        Latitude = p.Latitude,
        Longitude = p.Longitude,
        CapacidadeKg = p.CapacidadeKg,
        OcupacaoAtual = p.OcupacaoAtual,
        PercentualOcupacao = p.PercentualOcupacao,
        CapacidadeCritica = p.CapacidadeCritica,
        Ativo = p.Ativo,
        UsuarioId = p.UsuarioId,
        UsuarioNome = p.Usuario?.Nome,
        DataCriacao = p.DataCriacao
    };
}
