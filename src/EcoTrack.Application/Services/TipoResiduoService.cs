using EcoTrack.Application.Exceptions;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Interfaces;

namespace EcoTrack.Application.Services;

public class TipoResiduoService : ITipoResiduoService
{
    private readonly ITipoResiduoRepository _tipoResiduoRepository;

    public TipoResiduoService(ITipoResiduoRepository tipoResiduoRepository)
    {
        _tipoResiduoRepository = tipoResiduoRepository;
    }

    public async Task<List<TipoResiduoVM>> ListarAtivosAsync()
    {
        var tipos = await _tipoResiduoRepository.GetAtivosAsync();
        return tipos.Select(MapToVM).ToList();
    }

    public async Task<TipoResiduoVM> BuscarPorIdAsync(int id)
    {
        var tipo = await _tipoResiduoRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Tipo de resíduo", id);
        return MapToVM(tipo);
    }

    public async Task<TipoResiduoVM> CriarAsync(CriarTipoResiduoVM request)
    {
        if (await _tipoResiduoRepository.ExistsByNomeAsync(request.Nome))
            throw new BusinessRuleException($"Tipo de resíduo '{request.Nome}' já cadastrado.");

        var tipo = new TipoResiduo
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            CorLixeira = request.CorLixeira.ToUpper(),
            Instrucao = request.Instrucao
        };

        await _tipoResiduoRepository.AddAsync(tipo);
        return MapToVM(tipo);
    }

    public async Task<TipoResiduoVM> AtualizarAsync(int id, CriarTipoResiduoVM request)
    {
        var tipo = await _tipoResiduoRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Tipo de resíduo", id);

        tipo.Nome = request.Nome;
        tipo.Descricao = request.Descricao;
        tipo.CorLixeira = request.CorLixeira.ToUpper();
        tipo.Instrucao = request.Instrucao;

        await _tipoResiduoRepository.UpdateAsync(tipo);
        return MapToVM(tipo);
    }

    public async Task DesativarAsync(int id)
    {
        var tipo = await _tipoResiduoRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Tipo de resíduo", id);
        tipo.Ativo = false;
        await _tipoResiduoRepository.UpdateAsync(tipo);
    }

    private static TipoResiduoVM MapToVM(TipoResiduo t) => new()
    {
        Id = t.Id,
        Nome = t.Nome,
        Descricao = t.Descricao,
        CorLixeira = t.CorLixeira,
        Instrucao = t.Instrucao,
        Ativo = t.Ativo
    };
}
