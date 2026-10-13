using EcoTrack.Domain.Common;
using EcoTrack.Domain.Enums;

namespace EcoTrack.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; }

    public ICollection<PontoColeta> PontosColeta { get; set; } = new List<PontoColeta>();
    public ICollection<Coleta> Coletas { get; set; } = new List<Coleta>();
    public ICollection<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();
    public ICollection<Relatorio> Relatorios { get; set; } = new List<Relatorio>();
}
