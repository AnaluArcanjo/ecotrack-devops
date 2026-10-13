using EcoTrack.Domain.Common;
using EcoTrack.Domain.Enums;

namespace EcoTrack.Domain.Entities;

public class Notificacao : BaseEntity
{
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string Titulo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public TipoNotificacao Tipo { get; set; }
    public bool Lida { get; set; }
    public DateTime? DataLeitura { get; set; }
}
