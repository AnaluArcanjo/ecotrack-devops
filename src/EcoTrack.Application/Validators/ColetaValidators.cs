using EcoTrack.Application.ViewModels;
using FluentValidation;

namespace EcoTrack.Application.Validators;

public class AgendarColetaValidator : AbstractValidator<AgendarColetaVM>
{
    public AgendarColetaValidator()
    {
        RuleFor(x => x.PontoColetaId).GreaterThan(0)
            .WithMessage("Ponto de coleta é obrigatório.");

        RuleFor(x => x.TipoResiduoId).GreaterThan(0)
            .WithMessage("Tipo de resíduo é obrigatório.");

        RuleFor(x => x.QuantidadeKg)
            .GreaterThanOrEqualTo(0.1m).WithMessage("Quantidade deve ser maior que zero.")
            .LessThanOrEqualTo(10000m).WithMessage("Quantidade não pode exceder 10.000 kg.");

        RuleFor(x => x.DataAgendamento)
            .GreaterThan(DateTime.UtcNow).When(x => x.DataAgendamento.HasValue)
            .WithMessage("Data de agendamento deve ser futura.");

        RuleFor(x => x.Observacao).MaximumLength(500)
            .WithMessage("Observação deve ter no máximo 500 caracteres.");
    }
}

public class AtualizarColetaValidator : AbstractValidator<AtualizarColetaVM>
{
    private static readonly string[] StatusValidos = { "Pendente", "Agendada", "Coletada", "Cancelada" };

    public AtualizarColetaValidator()
    {
        RuleFor(x => x.Status)
            .Must(s => string.IsNullOrEmpty(s) || StatusValidos.Contains(s))
            .WithMessage($"Status deve ser um dos seguintes: {string.Join(", ", StatusValidos)}.");

        RuleFor(x => x.Observacao).MaximumLength(500);
    }
}
