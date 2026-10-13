using EcoTrack.Domain.Common;
using EcoTrack.Domain.Enums;

namespace EcoTrack.Domain.Entities;

public class Alerta : BaseEntity
{
    public int PontoColetaId { get; set; }
    public PontoColeta? PontoColeta { get; set; }

    public TipoAlerta Tipo { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public bool Resolvido { get; set; }
    public DateTime? DataResolucao { get; set; }
}
