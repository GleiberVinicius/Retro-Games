using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels.ApiModels;

namespace RetroGames.WebApi.Mapeadores;

public static class CondicaoJogoMapper
{
    public static ListarCondicaoJogoResponseApiModel ParaResponse(this CondicaoJogo condicao)
        => new(condicao.Id, condicao.Descricao);
}
