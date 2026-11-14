using RetroGames.Services.Modelos;
using RetroGames.WebApi.ApiModels.ApiModels;

namespace RetroGames.WebApi.Mapeadores;

public static class JogoMapper
{
    public static (string? Busca, int? IdPlataforma, int? IdGenero, int Pagina, int TamanhoPagina)
        ParaFiltro(this ListarJogoRequestApiModel consulta)
        => (consulta.Busca, consulta.IdPlataforma, consulta.IdGenero, consulta.Pagina, consulta.TamanhoPagina);

    public static ListarJogoResponseApiModel ParaResponse(this JogoDetalheServiceModel jogo)
        => new(jogo.Id, jogo.Nome, jogo.AnoLancamento, jogo.Preco, jogo.Estoque, jogo.Ativo,
            jogo.Plataforma, jogo.Condicao, jogo.Generos.Select(genero => genero.Nome).ToArray());

    public static ListarPorIdJogoResponseApiModel ParaResponseDetalhada(this JogoDetalheServiceModel jogo)
        => new(jogo.Id, jogo.Nome, jogo.Descricao, jogo.AnoLancamento, jogo.Preco, jogo.Estoque, jogo.Ativo,
            jogo.DataCadastro, jogo.IdPlataforma, jogo.Plataforma, jogo.IdCondicaoJogo, jogo.Condicao,
            jogo.Generos.Select(genero => new GeneroApiModel(genero.Id, genero.Nome)).ToArray());
}
