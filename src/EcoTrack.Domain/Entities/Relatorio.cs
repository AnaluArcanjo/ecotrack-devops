using EcoTrack.Domain.Common;

namespace EcoTrack.Domain.Entities;

public class Relatorio : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFim { get; set; }
    public decimal TotalKg { get; set; }
    public long TotalColetas { get; set; }
    public decimal Co2EvitadoKg { get; set; }

    public int GeradoPorId { get; set; }
    public Usuario? GeradoPor { get; set; }
}
