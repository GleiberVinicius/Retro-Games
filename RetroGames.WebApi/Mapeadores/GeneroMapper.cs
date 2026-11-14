using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels.ApiModels;

namespace RetroGames.WebApi.Mapeadores;

public static class GeneroMapper
{
    public static ListarGeneroResponseApiModel ParaResponse(this Genero genero)
        => new(genero.Id, genero.Nome);
}
