using Microsoft.AspNetCore.Http;
using Moq;
using RetroGames.Model.Exceptions;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Excecoes;
using RetroGames.Services.Servicos;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public sealed class PlataformaServiceTests
{
    [Fact]
    public async Task Inserir_NomeDuplicado_DeveLancarRegraDeNegocio()
    {
        // Arrange
        var plataformas = new Mock<IPlataformaRepository>();
        var repositorios = new Mock<IRepositorioSet>();
        repositorios.SetupGet(x => x.Plataformas).Returns(plataformas.Object);
        plataformas
            .Setup(x => x.ExisteNomeAsync("Nintendo", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var servico = new PlataformaService(repositorios.Object);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => servico.InserirAsync(
            new Plataforma { Nome = "Nintendo", Fabricante = "Nintendo", AnoLancamento = 1985 },
            TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO.Codigo, erro.Codigo);
        plataformas.Verify(
            x => x.InserirAsync(It.IsAny<Plataforma>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Inserir_DuplicidadeNaGravacao_DeveConverterExcecao()
    {
        // Arrange
        var plataformas = new Mock<IPlataformaRepository>();
        var repositorios = new Mock<IRepositorioSet>();
        repositorios.SetupGet(x => x.Plataformas).Returns(plataformas.Object);
        plataformas
            .Setup(x => x.ExisteNomeAsync("Nintendo", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        plataformas
            .Setup(x => x.InserirAsync(It.IsAny<Plataforma>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RegistroDuplicadoException("duplicado"));
        var servico = new PlataformaService(repositorios.Object);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => servico.InserirAsync(
            new Plataforma { Nome = "Nintendo" },
            TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO.Codigo, erro.Codigo);
    }

    [Fact]
    public async Task Inserir_NomeLivre_DeveRetornarPlataformaComIdDoRepositorio()
    {
        // Arrange
        const int idGerado = 42;
        var plataforma = new Plataforma
        {
            Nome = "Mega Drive",
            Fabricante = "Sega",
            AnoLancamento = 1988,
        };
        var plataformas = new Mock<IPlataformaRepository>();
        var repositorios = new Mock<IRepositorioSet>();
        repositorios.SetupGet(x => x.Plataformas).Returns(plataformas.Object);
        plataformas
            .Setup(x => x.ExisteNomeAsync("Mega Drive", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        plataformas
            .Setup(x => x.InserirAsync(plataforma, It.IsAny<CancellationToken>()))
            .ReturnsAsync(idGerado);
        var servico = new PlataformaService(repositorios.Object);

        // Act
        var resultado = await servico.InserirAsync(plataforma, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(idGerado, resultado.Id);
        Assert.Equal("Mega Drive", resultado.Nome);
        plataformas.Verify(x => x.InserirAsync(plataforma, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Obter_IdInexistente_DeveLancarErroDePlataformaNaoEncontrada()
    {
        // Arrange
        const int idInexistente = 999999;
        var plataformas = new Mock<IPlataformaRepository>();
        var repositorios = new Mock<IRepositorioSet>();
        repositorios.SetupGet(x => x.Plataformas).Returns(plataformas.Object);
        plataformas
            .Setup(x => x.ObterAsync(idInexistente, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plataforma?)null);
        var servico = new PlataformaService(repositorios.Object);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            servico.ObterAsync(idInexistente, TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA.Codigo, erro.Codigo);
        Assert.Equal(StatusCodes.Status404NotFound, erro.StatusHttp);
        plataformas.Verify(
            x => x.ObterAsync(idInexistente, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Atualizar_MantendoProprioNome_DeveGravarAlteracao()
    {
        // Arrange
        const int id = 7;
        var plataforma = new Plataforma
        {
            Id = id,
            Nome = "Atari 2600",
            Fabricante = "Atari",
            AnoLancamento = 1977,
        };
        var plataformas = new Mock<IPlataformaRepository>();
        var repositorios = new Mock<IRepositorioSet>();
        repositorios.SetupGet(x => x.Plataformas).Returns(plataformas.Object);
        plataformas
            .Setup(x => x.ObterAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Plataforma { Id = id, Nome = plataforma.Nome });
        plataformas
            .Setup(x => x.ExisteNomeAsync(plataforma.Nome, id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        plataformas
            .Setup(x => x.AtualizarAsync(plataforma, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var servico = new PlataformaService(repositorios.Object);

        // Act
        await servico.AtualizarAsync(plataforma, TestContext.Current.CancellationToken);

        // Assert
        plataformas.Verify(
            x => x.ExisteNomeAsync(plataforma.Nome, id, It.IsAny<CancellationToken>()),
            Times.Once);
        plataformas.Verify(x => x.AtualizarAsync(plataforma, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Excluir_ComJogosVinculados_DeveLancarErroENaoExcluir()
    {
        // Arrange
        const int id = 7;
        var plataformas = new Mock<IPlataformaRepository>();
        var repositorios = new Mock<IRepositorioSet>();
        repositorios.SetupGet(x => x.Plataformas).Returns(plataformas.Object);
        plataformas
            .Setup(x => x.ObterAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Plataforma { Id = id, Nome = "Atari 2600" });
        plataformas
            .Setup(x => x.PossuiJogosAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var servico = new PlataformaService(repositorios.Object);

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            servico.ExcluirAsync(id, TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_POSSUI_JOGOS.Codigo, erro.Codigo);
        plataformas.Verify(x => x.ExcluirAsync(id, It.IsAny<CancellationToken>()), Times.Never);
    }
}
