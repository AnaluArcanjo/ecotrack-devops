namespace EcoTrack.Application.ViewModels;

public class AgendarColetaVM
{
    public int PontoColetaId { get; set; }
    public int TipoResiduoId { get; set; }
    public decimal QuantidadeKg { get; set; }
    public DateTime? DataAgendamento { get; set; }
    public string? Observacao { get; set; }
}

public class AtualizarColetaVM
{
    public string? Status { get; set; }
    public string? Observacao { get; set; }
    public DateTime? DataAgendamento { get; set; }
    public DateTime? DataColeta { get; set; }
}

public class ColetaVM
{
    public int Id { get; set; }
    public int PontoColetaId { get; set; }
    public string? PontoColetaNome { get; set; }
    public int TipoResiduoId { get; set; }
    public string? TipoResiduoNome { get; set; }
    public string? TipoResiduoCor { get; set; }
    public int UsuarioId { get; set; }
    public string? UsuarioNome { get; set; }
    public decimal QuantidadeKg { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? DataAgendamento { get; set; }
    public DateTime? DataColeta { get; set; }
    public string? Observacao { get; set; }
    public DateTime DataCriacao { get; set; }
}
