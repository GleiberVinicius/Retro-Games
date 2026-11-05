using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;
namespace RetroGames.Services.Servicos;
public sealed class DominioService(IRepositorioSet repositorios) : IDominioService
{
    public Task<IReadOnlyList<Genero>> ListarGenerosAsync(CancellationToken ct=default)=>repositorios.Generos.ListarAsync(ct);
    public Task<IReadOnlyList<CondicaoJogo>> ListarCondicoesAsync(CancellationToken ct=default)=>repositorios.Condicoes.ListarAsync(ct);
}
