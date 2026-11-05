using FluentValidation;
using RetroGames.WebApi.ApiModels.ApiModels;
namespace RetroGames.WebApi.Validators;
public sealed class PlataformaEntradaValidator:AbstractValidator<PlataformaEntradaApiModel>
{ public PlataformaEntradaValidator(){RuleFor(x=>x.Nome).NotEmpty().WithMessage("Informe o nome.").MaximumLength(100).WithMessage("O nome deve ter até 100 caracteres.");RuleFor(x=>x.Fabricante).NotEmpty().WithMessage("Informe o fabricante.").MaximumLength(100);RuleFor(x=>x.AnoLancamento).InclusiveBetween((short)1970,(short)2100).WithMessage("Ano de lançamento inválido.");} }
