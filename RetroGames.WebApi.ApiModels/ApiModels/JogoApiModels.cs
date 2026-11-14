namespace RetroGames.WebApi.ApiModels.ApiModels;

public sealed class ListarJogoRequestApiModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
}

public sealed record ListarJogoResponseApiModel(
    int Id,
    string Nome,
    short AnoLancamento,
    decimal Preco,
    int Estoque,
    bool Ativo,
    string Plataforma,
    string Condicao,
    IReadOnlyList<string> Generos);

public sealed record ListarPorIdJogoResponseApiModel(
    int Id,
    string Nome,
    string? Descricao,
    short AnoLancamento,
    decimal Preco,
    int Estoque,
    bool Ativo,
    DateTime DataCadastro,
    int IdPlataforma,
    string Plataforma,
    int IdCondicaoJogo,
    string Condicao,
    IReadOnlyList<GeneroApiModel> Generos);

public sealed record GeneroApiModel(int Id, string Nome);
public sealed record PaginaResponseApiModel<T>(IReadOnlyList<T> Itens, int Pagina, int TamanhoPagina, int TotalItens, int TotalPaginas);

public sealed class JogoEntradaApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public short AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int IdPlataforma { get; set; }
    public int IdCondicaoJogo { get; set; }
    public List<int> IdsGeneros { get; set; } = [];
}
