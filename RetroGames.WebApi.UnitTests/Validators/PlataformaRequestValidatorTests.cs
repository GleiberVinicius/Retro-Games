using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Validators;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Validators;

public sealed class PlataformaRequestValidatorTests
{
    [Fact]
    public void Inserir_ComCamposValidos_DevePassar()
    {
        var resultado = new InserirPlataformaRequestValidator().Validate(new InserirPlataformaRequestApiModel
        { Nome = new string('N', 80), Fabricante = new string('F', 80), AnoLancamento = 1970 });

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public void Inserir_ComCamposObrigatoriosVazios_DeveFalhar()
    {
        var resultado = new InserirPlataformaRequestValidator().Validate(new InserirPlataformaRequestApiModel
        { Nome = " ", Fabricante = " ", AnoLancamento = 2000 });

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(InserirPlataformaRequestApiModel.Nome));
        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(InserirPlataformaRequestApiModel.Fabricante));
    }

    [Theory]
    [InlineData("Nome")]
    [InlineData("Fabricante")]
    public void Inserir_CampoAcimaDeOitentaCaracteres_DeveFalhar(string campo)
    {
        var modelo = new InserirPlataformaRequestApiModel { Nome = "Console", Fabricante = "Marca", AnoLancamento = 2000 };
        if (campo == "Nome") modelo.Nome = new string('N', 81);
        else modelo.Fabricante = new string('F', 81);

        var resultado = new InserirPlataformaRequestValidator().Validate(modelo);

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == campo);
    }

    [Fact]
    public void Alterar_ComCamposObrigatoriosVazios_DeveFalhar()
    {
        var resultado = new AlterarPlataformaRequestValidator().Validate(new AlterarPlataformaRequestApiModel
        { Nome = " ", Fabricante = " ", AnoLancamento = 2000 });

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(AlterarPlataformaRequestApiModel.Nome));
        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(AlterarPlataformaRequestApiModel.Fabricante));
    }

    [Theory]
    [InlineData("Nome")]
    [InlineData("Fabricante")]
    public void Alterar_CampoAcimaDeOitentaCaracteres_DeveFalhar(string campo)
    {
        var modelo = new AlterarPlataformaRequestApiModel { Nome = "Console", Fabricante = "Marca", AnoLancamento = 2000 };
        if (campo == "Nome") modelo.Nome = new string('N', 81);
        else modelo.Fabricante = new string('F', 81);

        var resultado = new AlterarPlataformaRequestValidator().Validate(modelo);

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == campo);
    }

    [Theory]
    [InlineData(1970)]
    [InlineData(2100)]
    public void AnoNosLimites_DeveSerValido(short ano)
    {
        var resultado = new AlterarPlataformaRequestValidator().Validate(new AlterarPlataformaRequestApiModel
        { Nome = "Console", Fabricante = "Marca", AnoLancamento = ano });

        Assert.True(resultado.IsValid);
    }

    [Theory]
    [InlineData(1800)]
    [InlineData(2101)]
    public void AnoForaDosLimites_DeveFalhar(short ano)
    {
        var resultado = new AlterarPlataformaRequestValidator().Validate(new AlterarPlataformaRequestApiModel
        { Nome = "Console", Fabricante = "Marca", AnoLancamento = ano });

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(AlterarPlataformaRequestApiModel.AnoLancamento));
    }
}
