namespace EcoTrack.Application.ViewModels;

public class CriarTipoResiduoVM
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string CorLixeira { get; set; } = string.Empty;
    public string Instrucao { get; set; } = string.Empty;
}

public class TipoResiduoVM
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string CorLixeira { get; set; } = string.Empty;
    public string Instrucao { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}

public class AlertaVM
{
    public int Id { get; set; }
    public int PontoColetaId { get; set; }
    public string? PontoColetaNome { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public bool Resolvido { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataResolucao { get; set; }
}

public class NotificacaoVM
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public bool Lida { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataLeitura { get; set; }
}

public class GerarRelatorioVM
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFim { get; set; }
}

public class RelatorioVM
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime PeriodoInicio { get; set; }
    public DateTime PeriodoFim { get; set; }
    public decimal TotalKg { get; set; }
    public long TotalColetas { get; set; }
    public decimal Co2EvitadoKg { get; set; }
    public string? GeradoPorNome { get; set; }
    public DateTime DataCriacao { get; set; }
}

public class RankingResiduoVM
{
    public string TipoResiduo { get; set; } = string.Empty;
    public decimal TotalKg { get; set; }
}

public class DashboardVM
{
    public int TotalPontosColeta { get; set; }
    public long TotalColetasRealizadas { get; set; }
    public decimal TotalResiduosKg { get; set; }
    public decimal Co2EvitadoTotalKg { get; set; }
    public int AlertasAbertos { get; set; }
    public int NotificacoesNaoLidas { get; set; }
    public List<RankingResiduoVM> RankingResiduos { get; set; } = new();
    public List<PontoColetaVM> PontosCriticos { get; set; } = new();
}
