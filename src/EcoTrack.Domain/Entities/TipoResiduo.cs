using EcoTrack.Domain.Common;

namespace EcoTrack.Domain.Entities;

public class TipoResiduo : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string CorLixeira { get; set; } = string.Empty;
    public string Instrucao { get; set; } = string.Empty;

    public ICollection<Coleta> Coletas { get; set; } = new List<Coleta>();
}
