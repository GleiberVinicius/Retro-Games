using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Moq;
using RetroGames.Model.Model;
using RetroGames.Services.Interfaces;
using RetroGames.Services.Modelos;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Controllers;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Controllers;

public sealed class PlataformaControllerTests
{
    [Fact]
    public void RotasDeGravacao_NaoDevemExigirLoginNestaEtapa()
    {
        Assert.DoesNotContain(typeof(PlataformaController).GetMethod(nameof(PlataformaController.Inserir))!.GetCustomAttributes(true), atributo => atributo is AuthorizeAttribute);
        Assert.DoesNotContain(typeof(PlataformaController).GetMethod(nameof(PlataformaController.Alterar))!.GetCustomAttributes(true), atributo => atributo is AuthorizeAttribute);
        Assert.DoesNotContain(typeof(PlataformaController).GetMethod(nameof(PlataformaController.Excluir))!.GetCustomAttributes(true), atributo => atributo is AuthorizeAttribute);
    }

    [Fact]
    public async Task Inserir_DeveRetornar201ComIdNaRotaELocacaoDoRecurso()
    {
        var servico = new Mock<IPlataformaService>();
        servico.Setup(x => x.InserirAsync(It.IsAny<Plataforma>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PlataformaServiceModel(12, "Mega Drive", "Sega", 1988));
        var controller = new PlataformaController(servico.Object);
        var modelo = new InserirPlataformaRequestApiModel
        { Nome = " Mega Drive ", Fabricante = " Sega ", AnoLancamento = 1988 };

        var resultado = Assert.IsType<CreatedAtActionResult>(await controller.Inserir(modelo, CancellationToken.None));

        Assert.Equal(nameof(PlataformaController.Obter), resultado.ActionName);
        Assert.Equal(12, resultado.RouteValues!["id"]);
        var envelope = Assert.IsType<EnvelopeResposta<ListarPlataformaResponseApiModel>>(resultado.Value);
        Assert.Equal(0, envelope.ResponseCode);
        Assert.Equal(new ListarPlataformaResponseApiModel(12, "Mega Drive", "Sega", 1988), envelope.Data);
        servico.Verify(x => x.InserirAsync(It.Is<Plataforma>(plataforma =>
            plataforma.Nome == "Mega Drive" && plataforma.Fabricante == "Sega"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Alterar_DeveRetornar200ComEnvelopeEIdDaRota()
    {
        var servico = new Mock<IPlataformaService>();
        servico.Setup(x => x.AtualizarAsync(It.IsAny<Plataforma>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var controller = new PlataformaController(servico.Object);

        var resultado = Assert.IsType<OkObjectResult>(await controller.Alterar(7,
            new AlterarPlataformaRequestApiModel { Nome = "Console", Fabricante = "Marca", AnoLancamento = 2000 },
            CancellationToken.None));

        var envelope = Assert.IsType<EnvelopeResposta<object>>(resultado.Value);
        Assert.Equal(0, envelope.ResponseCode);
        servico.Verify(x => x.AtualizarAsync(It.Is<Plataforma>(plataforma => plataforma.Id == 7), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Excluir_DeveRetornar200ComEnvelope()
    {
        var servico = new Mock<IPlataformaService>();
        servico.Setup(x => x.ExcluirAsync(9, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var controller = new PlataformaController(servico.Object);

        var resultado = Assert.IsType<OkObjectResult>(await controller.Excluir(9, CancellationToken.None));

        var envelope = Assert.IsType<EnvelopeResposta<object>>(resultado.Value);
        Assert.Equal(0, envelope.ResponseCode);
        servico.Verify(x => x.ExcluirAsync(9, It.IsAny<CancellationToken>()), Times.Once);
    }
}
