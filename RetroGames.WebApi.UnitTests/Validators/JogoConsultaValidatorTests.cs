using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Validators;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Validators;

public sealed class JogoConsultaValidatorTests
{
    private readonly JogoConsultaValidator _validador = new();

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 50)]
    public void PaginaETamanhoDentroDosLimites_SaoValidos(int pagina, int tamanhoPagina)
    {
        var resultado = _validador.Validate(new ListarJogoRequestApiModel
        {
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
        });

        Assert.True(resultado.IsValid);
    }

    [Theory]
    [InlineData(0, 12, nameof(ListarJogoRequestApiModel.Pagina))]
    [InlineData(1, 0, nameof(ListarJogoRequestApiModel.TamanhoPagina))]
    [InlineData(1, 51, nameof(ListarJogoRequestApiModel.TamanhoPagina))]
    public void PaginaOuTamanhoForaDosLimites_SaoInvalidos(int pagina, int tamanhoPagina, string propriedade)
    {
        var resultado = _validador.Validate(new ListarJogoRequestApiModel
        {
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
        });

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == propriedade);
    }
}
