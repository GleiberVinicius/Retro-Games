using RetroGames.Model.Model;
using RetroGames.Services.Modelos;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Mapeadores;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Mapeadores;

public sealed class JogoMapperTests
{
    [Fact]
    public void ParaResponse_DeveMontarItemDaListagemComNomesDosGeneros()
    {
        var jogo = CriarJogo();

        var resposta = jogo.ParaResponse();

        Assert.Equal(5, resposta.Id);
        Assert.Equal("Sonic", resposta.Nome);
        Assert.Equal((short)1991, resposta.AnoLancamento);
        Assert.Equal(45.50m, resposta.Preco);
        Assert.Equal(3, resposta.Estoque);
        Assert.True(resposta.Ativo);
        Assert.Equal("Mega Drive", resposta.Plataforma);
        Assert.Equal("Usado", resposta.Condicao);
        Assert.Equal(["Plataforma", "Ação"], resposta.Generos);
    }

    [Fact]
    public void ParaResponseDetalhada_DeveIncluirDescricaoEIdsRelacionados()
    {
        var jogo = CriarJogo();

        var resposta = jogo.ParaResponseDetalhada();

        Assert.Equal(5, resposta.Id);
        Assert.Equal("Sonic", resposta.Nome);
        Assert.Equal("O ouriço azul", resposta.Descricao);
        Assert.Equal((short)1991, resposta.AnoLancamento);
        Assert.Equal(45.50m, resposta.Preco);
        Assert.Equal(3, resposta.Estoque);
        Assert.True(resposta.Ativo);
        Assert.Equal(DateTime.UnixEpoch, resposta.DataCadastro);
        Assert.Equal(2, resposta.IdPlataforma);
        Assert.Equal("Mega Drive", resposta.Plataforma);
        Assert.Equal(4, resposta.IdCondicaoJogo);
        Assert.Equal("Usado", resposta.Condicao);
        Assert.Equal([new GeneroApiModel(8, "Plataforma"), new GeneroApiModel(9, "Ação")], resposta.Generos);
    }

    [Fact]
    public void ParaFiltro_DeveCopiarFiltrosEPaginacaoDaQuery()
    {
        var consulta = new ListarJogoRequestApiModel
        {
            Busca = "Sonic",
            IdPlataforma = 2,
            IdGenero = 8,
            Pagina = 2,
            TamanhoPagina = 12,
        };

        var filtro = consulta.ParaFiltro();

        Assert.Equal(("Sonic", 2, 8, 2, 12), filtro);
    }

    private static JogoDetalheServiceModel CriarJogo()
        => new(5, "Sonic", "O ouriço azul", 1991, 45.50m, 3, true, DateTime.UnixEpoch,
            2, "Mega Drive", 4, "Usado",
            [new Genero { Id = 8, Nome = "Plataforma" }, new Genero { Id = 9, Nome = "Ação" }]);
}
