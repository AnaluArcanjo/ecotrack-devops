using EcoTrack.Domain.Common;
using EcoTrack.Domain.Enums;

namespace EcoTrack.Domain.Entities;

public class Coleta : BaseEntity
{
    public int PontoColetaId { get; set; }
    public PontoColeta? PontoColeta { get; set; }

    public int TipoResiduoId { get; set; }
    public TipoResiduo? TipoResiduo { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public decimal QuantidadeKg { get; set; }
    public StatusColeta Status { get; set; } = StatusColeta.Pendente;
    public DateTime? DataAgendamento { get; set; }
    public DateTime? DataColeta { get; set; }
    public string? Observacao { get; set; }
}
