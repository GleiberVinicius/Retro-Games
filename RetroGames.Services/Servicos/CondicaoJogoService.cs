using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services.Servicos;

public sealed class CondicaoJogoService(IRepositorioSet repositorio) : ICondicaoJogoService
{
    public Task<IReadOnlyList<CondicaoJogo>> ListarAsync(CancellationToken ct = default) =>
        repositorio.Condicoes.ListarAsync(ct);
}
