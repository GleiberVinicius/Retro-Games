using RetroGames.Services.Modelos;
using RetroGames.WebApi.ApiModels.ApiModels;

namespace RetroGames.WebApi.Mapeadores;

public static class PlataformaMapper
{
    public static ListarPlataformaResponseApiModel ParaResponse(this PlataformaServiceModel plataforma)
        => new(plataforma.Id, plataforma.Nome, plataforma.Fabricante, plataforma.AnoLancamento);
}
