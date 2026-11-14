using RetroGames.Services.Modelos;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Mapeadores;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Mapeadores;

public sealed class PlataformaMapperTests
{
    [Fact]
    public void ParaResponse_DeveConverterModeloDeServicoEmApiModel()
    {
        var plataforma = new PlataformaServiceModel(7, "Mega Drive", "Sega", 1988);

        var resposta = plataforma.ParaResponse();

        Assert.Equal(new ListarPlataformaResponseApiModel(7, "Mega Drive", "Sega", 1988), resposta);
    }
}
