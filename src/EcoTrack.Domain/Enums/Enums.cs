namespace EcoTrack.Domain.Enums;

public enum PerfilUsuario
{
    Admin = 1,
    Empresa = 2,
    Coletor = 3,
    Cidadao = 4
}

public enum StatusColeta
{
    Pendente = 1,
    Agendada = 2,
    Coletada = 3,
    Cancelada = 4
}

public enum TipoAlerta
{
    Capacidade = 1,
    Manutencao = 2,
    Irregularidade = 3
}

public enum TipoNotificacao
{
    Info = 1,
    Alerta = 2,
    Sucesso = 3,
    Erro = 4
}
