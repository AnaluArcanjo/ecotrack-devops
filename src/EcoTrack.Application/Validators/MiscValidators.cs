using EcoTrack.Application.ViewModels;
using FluentValidation;

namespace EcoTrack.Application.Validators;

public class CriarTipoResiduoValidator : AbstractValidator<CriarTipoResiduoVM>
{
    public CriarTipoResiduoValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descricao).MaximumLength(500);
        RuleFor(x => x.CorLixeira).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Instrucao).NotEmpty().MaximumLength(1000);
    }
}

public class GerarRelatorioValidator : AbstractValidator<GerarRelatorioVM>
{
    public GerarRelatorioValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Descricao).MaximumLength(1000);
        RuleFor(x => x.PeriodoInicio).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Data início deve ser passada ou presente.");
        RuleFor(x => x.PeriodoFim).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Data fim deve ser passada ou presente.")
            .GreaterThanOrEqualTo(x => x.PeriodoInicio)
            .WithMessage("Data fim deve ser posterior à data início.");
    }
}
