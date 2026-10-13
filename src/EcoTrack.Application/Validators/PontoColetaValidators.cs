using EcoTrack.Application.ViewModels;
using FluentValidation;

namespace EcoTrack.Application.Validators;

public class CriarPontoColetaValidator : AbstractValidator<CriarPontoColetaVM>
{
    public CriarPontoColetaValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Endereco).NotEmpty().WithMessage("Endereço é obrigatório.")
            .MaximumLength(255).WithMessage("Endereço deve ter no máximo 255 caracteres.");

        RuleFor(x => x.Cidade).NotEmpty().WithMessage("Cidade é obrigatória.")
            .MaximumLength(100);

        RuleFor(x => x.Estado).NotEmpty().WithMessage("Estado é obrigatório.")
            .Length(2).WithMessage("Estado deve ter 2 caracteres (UF).");

        RuleFor(x => x.Cep).NotEmpty().WithMessage("CEP é obrigatório.")
            .Matches(@"^\d{5}-\d{3}$").WithMessage("CEP deve estar no formato 00000-000.");

        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue)
            .WithMessage("Latitude inválida.");

        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue)
            .WithMessage("Longitude inválida.");

        RuleFor(x => x.CapacidadeKg).GreaterThan(0)
            .WithMessage("Capacidade deve ser maior que zero.");
    }
}

public class AtualizarOcupacaoValidator : AbstractValidator<AtualizarOcupacaoVM>
{
    public AtualizarOcupacaoValidator()
    {
        RuleFor(x => x.OcupacaoAtual).GreaterThanOrEqualTo(0)
            .WithMessage("Ocupação não pode ser negativa.");
    }
}
