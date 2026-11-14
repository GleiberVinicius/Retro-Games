using FluentValidation;
using RetroGames.WebApi.ApiModels.ApiModels;
namespace RetroGames.WebApi.Validators;
public sealed class JogoConsultaValidator:AbstractValidator<ListarJogoRequestApiModel>
{ public JogoConsultaValidator(){RuleFor(x=>x.Pagina).GreaterThanOrEqualTo(1).WithMessage("A página deve ser maior ou igual a 1.");RuleFor(x=>x.TamanhoPagina).GreaterThanOrEqualTo(1).WithMessage("O tamanho da página deve ser maior ou igual a 1.");RuleFor(x=>x.IdPlataforma).GreaterThan(0).When(x=>x.IdPlataforma.HasValue).WithMessage("A plataforma deve ser válida.");RuleFor(x=>x.IdGenero).GreaterThan(0).When(x=>x.IdGenero.HasValue).WithMessage("O gênero deve ser válido.");} }
