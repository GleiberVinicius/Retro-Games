using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Validators;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Validators;

public sealed class JogoRequestValidatorTests
{
    [Fact]
    public void Inserir_ComValoresValidos_DevePassar()
    {
        var resultado = new InserirJogoRequestValidator().Validate(new InserirJogoRequestApiModel
        {
            Nome = "Sonic",
            Descricao = new string('D', 1000),
            AnoLancamento = 1991,
            Preco = 49.99m,
            Estoque = 0,
            IdPlataforma = 1,
            IdCondicaoJogo = 1,
            IdsGeneros = [1, 2],
        });

        Assert.True(resultado.IsValid);
    }

    [Theory]
    [InlineData(1970)]
    [InlineData(2100)]
    public void AnoNosLimites_DeveSerValido(short ano)
    {
        var resultado = new AlterarJogoRequestValidator().Validate(CriarAlteracao(ano: ano));

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public void Inserir_NomeVazioEDescricaoLonga_DeveFalhar()
    {
        var resultado = new InserirJogoRequestValidator().Validate(new InserirJogoRequestApiModel
        {
            Nome = " ", Descricao = new string('D', 1001), AnoLancamento = 1990,
            Preco = 1, Estoque = 0, IdPlataforma = 1, IdCondicaoJogo = 1, IdsGeneros = [1],
        });

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(InserirJogoRequestApiModel.Nome));
        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(InserirJogoRequestApiModel.Descricao));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1.234)]
    public void Alterar_PrecoForaDasRegras_DeveFalhar(decimal preco)
    {
        var resultado = new AlterarJogoRequestValidator().Validate(CriarAlteracao(preco: preco));

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(AlterarJogoRequestApiModel.Preco));
    }

    [Fact]
    public void Alterar_PrecoComMaisDeDuasCasasIncluindoZero_DeveFalhar()
    {
        var resultado = new AlterarJogoRequestValidator().Validate(CriarAlteracao(preco: 1.230m));

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(AlterarJogoRequestApiModel.Preco));
    }

    [Fact]
    public void Inserir_ReferenciasInvalidasOuSemGeneros_DeveFalhar()
    {
        var resultado = new InserirJogoRequestValidator().Validate(new InserirJogoRequestApiModel
        {
            Nome = "Jogo", AnoLancamento = 1990, Preco = 1, Estoque = 0,
            IdPlataforma = 0, IdCondicaoJogo = 0, IdsGeneros = [],
        });

        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(InserirJogoRequestApiModel.IdPlataforma));
        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(InserirJogoRequestApiModel.IdCondicaoJogo));
        Assert.Contains(resultado.Errors, erro => erro.PropertyName == nameof(InserirJogoRequestApiModel.IdsGeneros));
    }

    private static AlterarJogoRequestApiModel CriarAlteracao(short ano = 1991, decimal preco = 49.99m) => new()
    {
        Nome = "Sonic", AnoLancamento = ano, Preco = preco, Estoque = 0,
        IdPlataforma = 1, IdCondicaoJogo = 1, IdsGeneros = [1], Ativo = true,
    };
}
