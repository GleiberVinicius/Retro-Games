using FluentValidation;
using RetroGames.WebApi.ApiModels.ApiModels;

namespace RetroGames.WebApi.Validators;

public sealed class InserirPlataformaRequestValidator : AbstractValidator<InserirPlataformaRequestApiModel>
{
    public InserirPlataformaRequestValidator()
    {
        RuleFor(modelo => modelo.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(80).WithMessage("O nome deve ter até 80 caracteres.");
        RuleFor(modelo => modelo.Fabricante)
            .NotEmpty().WithMessage("Informe o fabricante.")
            .MaximumLength(80).WithMessage("O fabricante deve ter até 80 caracteres.");
        RuleFor(modelo => modelo.AnoLancamento)
            .InclusiveBetween((short)1970, (short)2100).WithMessage("Ano de lançamento inválido.");
    }
}
