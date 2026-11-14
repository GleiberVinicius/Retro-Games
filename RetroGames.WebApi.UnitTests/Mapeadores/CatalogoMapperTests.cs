using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Mapeadores;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Mapeadores;

public sealed class CatalogoMapperTests
{
    [Fact]
    public void GeneroParaResponse_DeveConverterIdENome()
    {
        var genero = new Genero { Id = 4, Nome = "Arcade" };

        var resposta = genero.ParaResponse();

        Assert.Equal(new ListarGeneroResponseApiModel(4, "Arcade"), resposta);
    }

    [Fact]
    public void CondicaoParaResponse_DeveConverterIdEDescricao()
    {
        var condicao = new CondicaoJogo { Id = 2, Descricao = "Usado" };

        var resposta = condicao.ParaResponse();

        Assert.Equal(new ListarCondicaoJogoResponseApiModel(2, "Usado"), resposta);
    }
}
