using Microsoft.AspNetCore.Mvc;
using Moq;
using RetroGames.Model.Model;
using RetroGames.Services.Interfaces;
using RetroGames.Services.Modelos;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Controllers;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Controllers;

public sealed class JogoControllerTests
{
    [Fact]
    public async Task Inserir_DeveRetornar201ComGenerosNoDetalhe()
    {
        var servico = new Mock<IJogoService>();
        servico.Setup(x => x.InserirAsync(It.IsAny<Jogo>(), It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarDetalhe());
        var controller = new JogoController(servico.Object);
        var modelo = new InserirJogoRequestApiModel
        {
            Nome = "Sonic", AnoLancamento = 1991, Preco = 45.50m, Estoque = 3,
            IdPlataforma = 2, IdCondicaoJogo = 4, IdsGeneros = [8, 9],
        };

        var resultado = Assert.IsType<CreatedAtActionResult>(await controller.Inserir(modelo, CancellationToken.None));

        Assert.Equal(12, resultado.RouteValues!["id"]);
        var envelope = Assert.IsType<EnvelopeResposta<ListarPorIdJogoResponseApiModel>>(resultado.Value);
        Assert.Equal(2, envelope.Data!.Generos.Count);
        Assert.Equal(["Plataforma", "Ação"], envelope.Data.Generos.Select(genero => genero.Nome));
        servico.Verify(x => x.InserirAsync(It.Is<Jogo>(jogo => jogo.Nome == "Sonic"),
            It.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { 8, 9 })), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Alterar_DeveEnviarIdAtivoEGenerosSelecionadosAoServico()
    {
        var servico = new Mock<IJogoService>();
        servico.Setup(x => x.AtualizarAsync(It.IsAny<Jogo>(), It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var controller = new JogoController(servico.Object);
        var modelo = new AlterarJogoRequestApiModel
        {
            Nome = "Sonic", AnoLancamento = 1991, Preco = 45.50m, Estoque = 3,
            IdPlataforma = 2, IdCondicaoJogo = 4, IdsGeneros = [9], Ativo = false,
        };

        var resultado = Assert.IsType<OkObjectResult>(await controller.Alterar(12, modelo, CancellationToken.None));

        Assert.IsType<EnvelopeResposta<object>>(resultado.Value);
        servico.Verify(x => x.AtualizarAsync(It.Is<Jogo>(jogo => jogo.Id == 12 && !jogo.Ativo),
            It.Is<IReadOnlyCollection<int>>(ids => ids.SequenceEqual(new[] { 9 })), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Excluir_DeveSolicitarDesativacaoEmanterRespostaSemConteudo()
    {
        var servico = new Mock<IJogoService>();
        servico.Setup(x => x.DesativarAsync(12, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var controller = new JogoController(servico.Object);

        var resultado = Assert.IsType<NoContentResult>(await controller.Excluir(12, CancellationToken.None));

        Assert.Equal(204, resultado.StatusCode);
        servico.Verify(x => x.DesativarAsync(12, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static JogoDetalheServiceModel CriarDetalhe()
        => new(12, "Sonic", "O ouriço azul", 1991, 45.50m, 3, true, DateTime.UnixEpoch,
            2, "Mega Drive", 4, "Usado",
            [new Genero { Id = 8, Nome = "Plataforma" }, new Genero { Id = 9, Nome = "Ação" }]);
}
