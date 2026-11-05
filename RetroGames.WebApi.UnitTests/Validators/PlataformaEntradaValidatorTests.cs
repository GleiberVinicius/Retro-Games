using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Validators;
using Xunit;
namespace RetroGames.WebApi.UnitTests.Validators;
public sealed class PlataformaEntradaValidatorTests
{
    private readonly PlataformaEntradaValidator _validador = new();
    [Fact] public void PlataformaValida_NaoDeveTerErros() { var resultado=_validador.Validate(new PlataformaEntradaApiModel{Nome="Super Nintendo",Fabricante="Nintendo",AnoLancamento=1990}); Assert.True(resultado.IsValid); }
    [Fact] public void NomeVazio_DeveFalhar() { var resultado=_validador.Validate(new PlataformaEntradaApiModel{Nome=" ",Fabricante="Nintendo",AnoLancamento=1990}); Assert.Contains(resultado.Errors,x=>x.PropertyName==nameof(PlataformaEntradaApiModel.Nome)); }
    [Fact] public void AnoForaDaFaixa_DeveFalhar() { var resultado=_validador.Validate(new PlataformaEntradaApiModel{Nome="Console",Fabricante="Marca",AnoLancamento=1969}); Assert.Contains(resultado.Errors,x=>x.PropertyName==nameof(PlataformaEntradaApiModel.AnoLancamento)); }
}
