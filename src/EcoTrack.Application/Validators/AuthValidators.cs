using EcoTrack.Application.ViewModels;
using FluentValidation;

namespace EcoTrack.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestVM>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email deve ser válido.");
        RuleFor(x => x.Senha).NotEmpty().WithMessage("Senha é obrigatória.");
    }
}

public class CadastroRequestValidator : AbstractValidator<CadastroRequestVM>
{
    private static readonly string[] PerfisValidos = { "Empresa", "Coletor", "Cidadao" };

    public CadastroRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome é obrigatório.")
            .Length(3, 100).WithMessage("Nome deve ter entre 3 e 100 caracteres.");

        RuleFor(x => x.Email).NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email deve ser válido.");

        RuleFor(x => x.Senha).NotEmpty().WithMessage("Senha é obrigatória.")
            .Length(6, 50).WithMessage("Senha deve ter entre 6 e 50 caracteres.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$")
            .WithMessage("Senha deve conter ao menos uma letra minúscula, uma maiúscula e um número.");

        RuleFor(x => x.Perfil).NotEmpty().WithMessage("Perfil é obrigatório.")
            .Must(p => PerfisValidos.Contains(p))
            .WithMessage($"Perfil deve ser um dos seguintes: {string.Join(", ", PerfisValidos)}.");
    }
}
