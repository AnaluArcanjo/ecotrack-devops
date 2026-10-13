using EcoTrack.Domain.Common;

namespace EcoTrack.Domain.Entities;

public class PontoColeta : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal CapacidadeKg { get; set; }
    public decimal OcupacaoAtual { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public ICollection<Coleta> Coletas { get; set; } = new List<Coleta>();
    public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();

    public decimal PercentualOcupacao =>
        CapacidadeKg <= 0 ? 0 : Math.Round((OcupacaoAtual / CapacidadeKg) * 100, 2);

    public bool CapacidadeCritica => PercentualOcupacao >= 80;
}
