namespace EcoTrack.Application.ViewModels;

public class LoginRequestVM
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class CadastroRequestVM
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty; // Empresa, Coletor, Cidadao
}

public class AuthResponseVM
{
    public string Token { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Bearer";
    public int UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
}
