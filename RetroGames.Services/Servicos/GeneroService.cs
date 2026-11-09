using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;

namespace RetroGames.Services.Servicos;

public sealed class GeneroService(IRepositorioSet repositorio) : IGeneroService
{
    public Task<IReadOnlyList<Genero>> ListarAsync(CancellationToken ct = default) =>
        repositorio.Generos.ListarAsync(ct);
}
