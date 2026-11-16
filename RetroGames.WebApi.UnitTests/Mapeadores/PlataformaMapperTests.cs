using RetroGames.Services.Modelos;
using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Mapeadores;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Mapeadores;

public sealed class PlataformaMapperTests
{
    [Fact]
    public void ParaModel_DeInsercao_DeveCriarPlataformaSemIdInformado()
    {
        var apiModel = new InserirPlataformaRequestApiModel
        {
            Nome = " Mega Drive ",
            Fabricante = " Sega ",
            AnoLancamento = 1988,
        };

        var plataforma = apiModel.ParaModel();

        Assert.Equal(0, plataforma.Id);
        Assert.Equal("Mega Drive", plataforma.Nome);
        Assert.Equal("Sega", plataforma.Fabricante);
        Assert.Equal((short)1988, plataforma.AnoLancamento);
    }

    [Fact]
    public void ParaModel_DeAlteracao_DeveUsarIdDaRota()
    {
        var apiModel = new AlterarPlataformaRequestApiModel
        {
            Nome = "Mega Drive 2",
            Fabricante = "Sega",
            AnoLancamento = 1990,
        };

        var plataforma = apiModel.ParaModel(12);

        Assert.Equal(12, plataforma.Id);
        Assert.Equal("Mega Drive 2", plataforma.Nome);
        Assert.Equal("Sega", plataforma.Fabricante);
        Assert.Equal((short)1990, plataforma.AnoLancamento);
    }

    [Fact]
    public void ParaResponse_DeveConverterModeloDeServicoEmApiModel()
    {
        var plataforma = new PlataformaServiceModel(7, "Mega Drive", "Sega", 1988);

        var resposta = plataforma.ParaResponse();

        Assert.Equal(new ListarPlataformaResponseApiModel(7, "Mega Drive", "Sega", 1988), resposta);
    }
}
