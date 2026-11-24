using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Servicos;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Services;

public sealed class GeneroServiceTests
{
    [Fact]
    public async Task Listar_DeveRetornarGenerosDoRepositorio()
    {
        // Arrange
        IReadOnlyList<Genero> generosEsperados =
        [
            new Genero { Id = 1, Nome = "Aventura" },
            new Genero { Id = 2, Nome = "RPG" },
        ];
        var generos = new Mock<IGeneroRepository>();
        var repositorios = new Mock<IRepositorioSet>();
        repositorios.SetupGet(x => x.Generos).Returns(generos.Object);
        generos
            .Setup(x => x.ListarAsync(TestContext.Current.CancellationToken))
            .ReturnsAsync(generosEsperados);
        var servico = new GeneroService(repositorios.Object);

        // Act
        var resultado = await servico.ListarAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Same(generosEsperados, resultado);
        generos.Verify(x => x.ListarAsync(TestContext.Current.CancellationToken), Times.Once);
    }
}
