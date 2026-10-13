using BCrypt.Net;
using EcoTrack.Application.Exceptions;
using EcoTrack.Application.Interfaces;
using EcoTrack.Application.ViewModels;
using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Enums;
using EcoTrack.Domain.Interfaces;

namespace EcoTrack.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUsuarioRepository usuarioRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _usuarioRepository = usuarioRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseVM> LoginAsync(LoginRequestVM request)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            throw new UnauthorizedAppException("Email ou senha inválidos.");

        if (!usuario.Ativo)
            throw new UnauthorizedAppException("Usuário inativo. Contate o administrador.");

        var token = _jwtTokenGenerator.GerarToken(
            usuario.Id, usuario.Email, usuario.Perfil.ToString(), out var expiraEm);

        return new AuthResponseVM
        {
            Token = token,
            Tipo = "Bearer",
            UsuarioId = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil.ToString(),
            ExpiraEm = expiraEm
        };
    }

    public async Task<AuthResponseVM> CadastrarAsync(CadastroRequestVM request)
    {
        if (await _usuarioRepository.ExistsByEmailAsync(request.Email))
            throw new BusinessRuleException($"Email '{request.Email}' já está em uso.");

        if (!Enum.TryParse<PerfilUsuario>(request.Perfil, true, out var perfil) || perfil == PerfilUsuario.Admin)
            throw new BusinessRuleException("Perfil inválido. Use: Empresa, Coletor ou Cidadao.");

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha, workFactor: 11),
            Perfil = perfil
        };

        await _usuarioRepository.AddAsync(usuario);

        var token = _jwtTokenGenerator.GerarToken(
            usuario.Id, usuario.Email, usuario.Perfil.ToString(), out var expiraEm);

        return new AuthResponseVM
        {
            Token = token,
            Tipo = "Bearer",
            UsuarioId = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil.ToString(),
            ExpiraEm = expiraEm
        };
    }
}
