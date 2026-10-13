namespace EcoTrack.Application.ViewModels;

public class CriarPontoColetaVM
{
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal CapacidadeKg { get; set; }
}

public class AtualizarOcupacaoVM
{
    public decimal OcupacaoAtual { get; set; }
}

public class PontoColetaVM
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal CapacidadeKg { get; set; }
    public decimal OcupacaoAtual { get; set; }
    public decimal PercentualOcupacao { get; set; }
    public bool CapacidadeCritica { get; set; }
    public bool Ativo { get; set; }
    public int UsuarioId { get; set; }
    public string? UsuarioNome { get; set; }
    public DateTime DataCriacao { get; set; }
}
