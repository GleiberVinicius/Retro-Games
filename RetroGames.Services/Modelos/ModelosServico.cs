using RetroGames.Model.Model;
namespace RetroGames.Services.Modelos;
public sealed record PlataformaServiceModel(int Id,string Nome,string Fabricante,short AnoLancamento)
{ public static PlataformaServiceModel De(Plataforma p)=>new(p.Id,p.Nome,p.Fabricante,p.AnoLancamento); }
public sealed record JogoResumoServiceModel(Jogo Jogo,string Plataforma,string Condicao,IReadOnlyList<Genero> Generos);
public sealed record JogoDetalheServiceModel(int Id,string Nome,string? Descricao,short AnoLancamento,decimal Preco,int Estoque,bool Ativo,DateTime DataCadastro,int IdPlataforma,string Plataforma,int IdCondicaoJogo,string Condicao,IReadOnlyList<Genero> Generos);
public sealed record PaginaServiceModel<T>(IReadOnlyList<T> Itens,int Pagina,int TamanhoPagina,int TotalItens);
