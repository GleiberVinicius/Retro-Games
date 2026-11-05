using FluentValidation;
using RetroGames.WebApi.ApiModels.ApiModels;
namespace RetroGames.WebApi.Validators;
public sealed class LoginValidator:AbstractValidator<LoginApiModel>
{ public LoginValidator(){RuleFor(x=>x.Email).NotEmpty().WithMessage("Informe o e-mail.").EmailAddress().WithMessage("Informe um e-mail válido.").MaximumLength(254);RuleFor(x=>x.Senha).NotEmpty().WithMessage("Informe a senha.").MaximumLength(256);} }
