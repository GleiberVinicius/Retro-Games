using FluentValidation;
using RetroGames.WebApi.ApiModels.ApiModels;

namespace RetroGames.WebApi.Validators;

public sealed class InserirJogoRequestValidator : AbstractValidator<InserirJogoRequestApiModel>
{
    public InserirJogoRequestValidator()
    {
        RuleFor(modelo => modelo.Nome)
            .NotEmpty().WithMessage("Informe o nome do jogo.")
            .MaximumLength(150).WithMessage("O nome deve ter até 150 caracteres.");
        RuleFor(modelo => modelo.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter até 1000 caracteres.");
        RuleFor(modelo => modelo.AnoLancamento)
            .InclusiveBetween((short)1970, (short)2100).WithMessage("Ano de lançamento deve estar entre 1970 e 2100.");
        RuleFor(modelo => modelo.Preco)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.")
            .PrecisionScale(10, 2, false).WithMessage("O preço deve ter até duas casas decimais.");
        RuleFor(modelo => modelo.Estoque)
            .GreaterThanOrEqualTo(0).WithMessage("O estoque não pode ser negativo.");
        RuleFor(modelo => modelo.IdPlataforma)
            .GreaterThan(0).WithMessage("Informe uma plataforma válida.");
        RuleFor(modelo => modelo.IdCondicaoJogo)
            .GreaterThan(0).WithMessage("Informe uma condição válida.");
        RuleFor(modelo => modelo.IdsGeneros)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Informe ao menos um gênero.")
            .Must(ids => ids is not null && ids.Distinct().Count() == ids.Count)
            .WithMessage("Não repita gêneros.");
    }
}
