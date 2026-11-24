using Microsoft.AspNetCore.Http;
using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Excecoes;
using RetroGames.Services.Servicos;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public sealed class JogoServiceTests
{
    [Fact]
    public async Task Inserir_ComPlataformaInexistente_DeveLancarErroENaoGravar()
    {
        // Arrange
        const int idPlataformaInexistente = 999;
        var repositorios = CriarRepositorios();
        repositorios.Plataformas
            .Setup(x => x.ObterAsync(idPlataformaInexistente, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plataforma?)null);
        var servico = new JogoService(repositorios.Conjunto.Object);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => servico.InserirAsync(
            CriarJogo(idPlataforma: idPlataformaInexistente),
            [3],
            TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA.Codigo, erro.Codigo);
        repositorios.Jogos.Verify(
            x => x.InserirAsync(It.IsAny<Jogo>(), It.IsAny<CancellationToken>()),
            Times.Never);
        repositorios.JogosGeneros.Verify(
            x => x.InserirAsync(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Inserir_ComCondicaoInexistente_DeveLancarErroENaoGravar()
    {
        // Arrange
        const int idCondicaoInexistente = 999;
        var repositorios = CriarRepositorios();
        ConfigurarPlataformaExistente(repositorios);
        repositorios.Condicoes
            .Setup(x => x.ExisteAsync(idCondicaoInexistente, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var servico = new JogoService(repositorios.Conjunto.Object);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => servico.InserirAsync(
            CriarJogo(idCondicao: idCondicaoInexistente),
            [3],
            TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(BusinessErrors.CONDICAO_NAO_ENCONTRADA.Codigo, erro.Codigo);
        repositorios.Jogos.Verify(
            x => x.InserirAsync(It.IsAny<Jogo>(), It.IsAny<CancellationToken>()),
            Times.Never);
        repositorios.JogosGeneros.Verify(
            x => x.InserirAsync(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Inserir_ComGeneroInexistente_DeveLancarErroENaoGravar()
    {
        // Arrange
        const int idGeneroInexistente = 999;
        var repositorios = CriarRepositorios();
        ConfigurarPlataformaExistente(repositorios);
        ConfigurarCondicaoExistente(repositorios);
        repositorios.Generos
            .Setup(x => x.ListarPorIdsAsync(
                It.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { idGeneroInexistente })),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Genero>());
        var servico = new JogoService(repositorios.Conjunto.Object);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => servico.InserirAsync(
            CriarJogo(),
            [idGeneroInexistente],
            TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(BusinessErrors.GENERO_NAO_ENCONTRADO.Codigo, erro.Codigo);
        repositorios.Jogos.Verify(
            x => x.InserirAsync(It.IsAny<Jogo>(), It.IsAny<CancellationToken>()),
            Times.Never);
        repositorios.JogosGeneros.Verify(
            x => x.InserirAsync(It.IsAny<int>(), It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Listar_ComFiltroDeGenero_DeveEnviarIdsDosJogosAoRepositorio()
    {
        // Arrange
        const int idGeneroFiltrado = 5;
        var repositorios = CriarRepositorios();
        repositorios.JogosGeneros
            .Setup(x => x.ListarTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new JogoGenero { IdJogo = 21, IdGenero = idGeneroFiltrado },
                new JogoGenero { IdJogo = 22, IdGenero = idGeneroFiltrado },
                new JogoGenero { IdJogo = 30, IdGenero = 8 },
            });
        var servico = new JogoService(repositorios.Conjunto.Object);

        // Act
        await servico.ListarAsync("Mario", 4, idGeneroFiltrado, 2, 10, false,
            TestContext.Current.CancellationToken);

        // Assert
        repositorios.Jogos.Verify(x => x.ListarAsync(
            "Mario", 4, false, 2, 10,
            It.Is<IReadOnlyCollection<int>?>(ids => ids != null
                && ids.OrderBy(id => id).SequenceEqual(new[] { 21, 22 })),
            It.IsAny<CancellationToken>()), Times.Once);
        repositorios.Jogos.Verify(x => x.ContarAsync(
            "Mario", 4, false,
            It.Is<IReadOnlyCollection<int>?>(ids => ids != null
                && ids.OrderBy(id => id).SequenceEqual(new[] { 21, 22 })),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Listar_SemGenero_DeveEnviarIdsNulosENaoBuscarVinculosParaFiltrar()
    {
        // Arrange
        var repositorios = CriarRepositorios();
        var servico = new JogoService(repositorios.Conjunto.Object);

        // Act
        await servico.ListarAsync(null, null, null, 1, 12, false,
            TestContext.Current.CancellationToken);

        // Assert
        repositorios.Jogos.Verify(x => x.ListarAsync(
            null, null, false, 1, 12,
            It.Is<IReadOnlyCollection<int>?>(ids => ids == null),
            It.IsAny<CancellationToken>()), Times.Once);
        repositorios.Jogos.Verify(x => x.ContarAsync(
            null, null, false,
            It.Is<IReadOnlyCollection<int>?>(ids => ids == null),
            It.IsAny<CancellationToken>()), Times.Once);
        repositorios.JogosGeneros.Verify(
            x => x.ListarTodosAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Listar_ComJogos_DeveMontarNomeDaPlataformaEDescricaoDaCondicao()
    {
        // Arrange
        var jogo = CriarJogo();
        var repositorios = CriarRepositorios();
        repositorios.Jogos
            .Setup(x => x.ListarAsync(null, null, false, 1, 12, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { jogo });
        repositorios.Jogos
            .Setup(x => x.ContarAsync(null, null, false, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        repositorios.Plataformas
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new Plataforma { Id = jogo.IdPlataforma, Nome = "Atari 2600" } });
        repositorios.Condicoes
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new CondicaoJogo { Id = jogo.IdCondicaoJogo, Descricao = "Usado" } });
        repositorios.Generos
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new Genero { Id = 3, Nome = "Aventura" } });
        repositorios.JogosGeneros
            .Setup(x => x.ListarPorJogosAsync(
                It.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { jogo.Id })),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new JogoGenero { IdJogo = jogo.Id, IdGenero = 3 } });
        var servico = new JogoService(repositorios.Conjunto.Object);

        // Act
        var resultado = await servico.ListarAsync(null, null, null, 1, 12, false,
            TestContext.Current.CancellationToken);

        // Assert
        var item = Assert.Single(resultado.Itens);
        Assert.Equal("Atari 2600", item.Plataforma);
        Assert.Equal("Usado", item.Condicao);
        Assert.Equal("Aventura", Assert.Single(item.Generos).Nome);
    }

    [Fact]
    public async Task Obter_IdInexistente_DeveLancarJogoNaoEncontrado()
    {
        // Arrange
        const int idInexistente = 999999;
        var repositorios = CriarRepositorios();
        repositorios.Jogos
            .Setup(x => x.ObterAsync(idInexistente, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Jogo?)null);
        var servico = new JogoService(repositorios.Conjunto.Object);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            servico.ObterAsync(idInexistente, false, TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO.Codigo, erro.Codigo);
        Assert.Equal(StatusCodes.Status404NotFound, erro.StatusHttp);
    }

    [Fact]
    public async Task Desativar_IdInexistente_DeveLancarJogoNaoEncontradoENaoDesativar()
    {
        // Arrange
        const int idInexistente = 999999;
        var repositorios = CriarRepositorios();
        repositorios.Jogos
            .Setup(x => x.ObterAsync(idInexistente, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Jogo?)null);
        var servico = new JogoService(repositorios.Conjunto.Object);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            servico.DesativarAsync(idInexistente, TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO.Codigo, erro.Codigo);
        repositorios.Jogos.Verify(
            x => x.DesativarAsync(idInexistente, It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static RepositoriosMockados CriarRepositorios()
    {
        var conjunto = new Mock<IRepositorioSet>();
        var jogos = new Mock<IJogoRepository>();
        var plataformas = new Mock<IPlataformaRepository>();
        var condicoes = new Mock<ICondicaoJogoRepository>();
        var generos = new Mock<IGeneroRepository>();
        var jogosGeneros = new Mock<IJogoGeneroRepository>();

        conjunto.SetupGet(x => x.Jogos).Returns(jogos.Object);
        conjunto.SetupGet(x => x.Plataformas).Returns(plataformas.Object);
        conjunto.SetupGet(x => x.Condicoes).Returns(condicoes.Object);
        conjunto.SetupGet(x => x.Generos).Returns(generos.Object);
        conjunto.SetupGet(x => x.JogosGeneros).Returns(jogosGeneros.Object);

        jogos
            .Setup(x => x.ListarAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<bool>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IReadOnlyCollection<int>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Jogo>());
        jogos
            .Setup(x => x.ContarAsync(It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<bool>(),
                It.IsAny<IReadOnlyCollection<int>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        plataformas
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Plataforma>());
        condicoes
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<CondicaoJogo>());
        generos
            .Setup(x => x.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Genero>());
        generos
            .Setup(x => x.ListarPorIdsAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Genero>());
        jogosGeneros
            .Setup(x => x.ListarTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<JogoGenero>());
        jogosGeneros
            .Setup(x => x.ListarPorJogosAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<JogoGenero>());

        return new(conjunto, jogos, plataformas, condicoes, generos, jogosGeneros);
    }

    private static void ConfigurarPlataformaExistente(RepositoriosMockados repositorios)
    {
        repositorios.Plataformas
            .Setup(x => x.ObterAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) => new Plataforma { Id = id, Nome = "Atari 2600" });
    }

    private static void ConfigurarCondicaoExistente(RepositoriosMockados repositorios)
    {
        repositorios.Condicoes
            .Setup(x => x.ExisteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private static Jogo CriarJogo(int idPlataforma = 7, int idCondicao = 2)
        => new()
        {
            Id = 25,
            Nome = "Adventure",
            Descricao = "Jogo de aventura",
            AnoLancamento = 1980,
            Preco = 89.90m,
            Estoque = 5,
            Ativo = true,
            DataCadastro = new DateTime(2026, 1, 1),
            IdPlataforma = idPlataforma,
            IdCondicaoJogo = idCondicao,
        };

    private sealed record RepositoriosMockados(
        Mock<IRepositorioSet> Conjunto,
        Mock<IJogoRepository> Jogos,
        Mock<IPlataformaRepository> Plataformas,
        Mock<ICondicaoJogoRepository> Condicoes,
        Mock<IGeneroRepository> Generos,
        Mock<IJogoGeneroRepository> JogosGeneros);
}
