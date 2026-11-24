using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Servicos;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public sealed class CondicaoJogoServiceTests
{
    [Fact]
    public async Task Listar_DeveRetornarCondicoesDoRepositorio()
    {
        // Arrange
        IReadOnlyList<CondicaoJogo> condicoesEsperadas =
        [
            new CondicaoJogo { Id = 1, Descricao = "Novo" },
            new CondicaoJogo { Id = 2, Descricao = "Usado" },
        ];
        var condicoes = new Mock<ICondicaoJogoRepository>();
        var repositorios = new Mock<IRepositorioSet>();
        repositorios.SetupGet(x => x.Condicoes).Returns(condicoes.Object);
        condicoes
            .Setup(x => x.ListarAsync(TestContext.Current.CancellationToken))
            .ReturnsAsync(condicoesEsperadas);
        var servico = new CondicaoJogoService(repositorios.Object);

        // Act
        var resultado = await servico.ListarAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(condicoesEsperadas, resultado);
        condicoes.Verify(x => x.ListarAsync(TestContext.Current.CancellationToken), Times.Once);
    }
}
