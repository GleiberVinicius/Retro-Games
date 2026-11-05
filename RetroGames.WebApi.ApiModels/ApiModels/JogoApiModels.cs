namespace RetroGames.WebApi.ApiModels.ApiModels;
public sealed record JogoApiModel(int Id, string Nome, string? Descricao, short AnoLancamento, decimal Preco, int Estoque, bool Ativo, DateTime DataCadastro, int IdPlataforma, string Plataforma, int IdCondicaoJogo, string Condicao, IReadOnlyList<GeneroApiModel> Generos);
public sealed record GeneroApiModel(int Id, string Nome);
public sealed record PaginaApiModel<T>(IReadOnlyList<T> Itens, int Pagina, int TamanhoPagina, int TotalItens, int TotalPaginas);
public sealed class JogoConsultaApiModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
    public bool IncluirInativos { get; set; }
}
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
