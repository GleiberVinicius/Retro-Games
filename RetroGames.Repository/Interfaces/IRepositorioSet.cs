using RetroGames.Model.Model;
namespace RetroGames.Repository.Interfaces;
public interface IRepositorioSet
{
    IPlataformaRepository Plataformas { get; }
    IJogoRepository Jogos { get; }
    IGeneroRepository Generos { get; }
    ICondicaoJogoRepository Condicoes { get; }
    IUsuarioRepository Usuarios { get; }
    IPerfilRepository Perfis { get; }
    IJogoGeneroRepository JogosGeneros { get; }
}
public interface IPlataformaRepository
{
    Task<IReadOnlyList<Plataforma>> ListarAsync(CancellationToken ct = default);
    Task<Plataforma?> ObterAsync(int id, CancellationToken ct = default);
    Task<bool> ExisteNomeAsync(string nome, int? ignorarId = null, CancellationToken ct = default);
    Task<bool> PossuiJogosAsync(int idPlataforma, CancellationToken ct = default);
    Task<int> InserirAsync(Plataforma item, CancellationToken ct = default);
    Task AtualizarAsync(Plataforma item, CancellationToken ct = default);
    Task ExcluirAsync(int id, CancellationToken ct = default);
}
public interface IJogoRepository
{
    Task<IReadOnlyList<Jogo>> ListarAsync(string? busca, int? idPlataforma, bool incluirInativos, int pagina, int tamanhoPagina, IReadOnlyCollection<int>? ids, CancellationToken ct = default);
    Task<int> ContarAsync(string? busca, int? idPlataforma, bool incluirInativos, IReadOnlyCollection<int>? ids, CancellationToken ct = default);
    Task<Jogo?> ObterAsync(int id, bool incluirInativo, CancellationToken ct = default);
    Task<int> InserirAsync(Jogo item, CancellationToken ct = default);
    Task AtualizarAsync(Jogo item, CancellationToken ct = default);
    Task DesativarAsync(int id, CancellationToken ct = default);
}
public interface IGeneroRepository { Task<IReadOnlyList<Genero>> ListarAsync(CancellationToken ct = default); Task<IReadOnlyList<Genero>> ListarPorIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct = default); }
public interface ICondicaoJogoRepository { Task<IReadOnlyList<CondicaoJogo>> ListarAsync(CancellationToken ct = default); Task<bool> ExisteAsync(int id, CancellationToken ct = default); }
public interface IJogoGeneroRepository
{
    Task<IReadOnlyList<JogoGenero>> ListarTodosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogos, CancellationToken ct = default);
    Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros, CancellationToken ct = default);
    Task RemoverPorJogoAsync(int idJogo, CancellationToken ct = default);
}
public interface IUsuarioRepository { Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default); Task InserirAsync(Usuario usuario, CancellationToken ct = default); }
public interface IPerfilRepository { Task<string?> ObterDescricaoAsync(int id, CancellationToken ct = default); }
