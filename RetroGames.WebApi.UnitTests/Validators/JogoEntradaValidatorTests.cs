using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Validators;
using Xunit;
namespace RetroGames.WebApi.UnitTests.Validators;
public sealed class JogoEntradaValidatorTests
{
    private readonly JogoEntradaValidator _validador=new();
    [Fact] public void JogoValido_NaoDeveTerErros(){var resultado=_validador.Validate(new JogoEntradaApiModel{Nome="Jogo",AnoLancamento=1995,Preco=10,Estoque=2,IdPlataforma=1,IdCondicaoJogo=1,IdsGeneros=[1]});Assert.True(resultado.IsValid);}
    [Fact] public void PrecoZerado_DeveFalhar(){var resultado=_validador.Validate(new JogoEntradaApiModel{Nome="Jogo",AnoLancamento=1995,Preco=0,Estoque=0,IdPlataforma=1,IdCondicaoJogo=1,IdsGeneros=[1]});Assert.Contains(resultado.Errors,x=>x.PropertyName==nameof(JogoEntradaApiModel.Preco));}
    [Fact] public void GenerosDuplicados_DeveFalhar(){var resultado=_validador.Validate(new JogoEntradaApiModel{Nome="Jogo",AnoLancamento=1995,Preco=1,Estoque=0,IdPlataforma=1,IdCondicaoJogo=1,IdsGeneros=[1,1]});Assert.Contains(resultado.Errors,x=>x.PropertyName==nameof(JogoEntradaApiModel.IdsGeneros));}
}
