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
}
