using RetroGames.Model.Exceptions;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Excecoes;
using RetroGames.Services.Interfaces;
using RetroGames.Services.Modelos;
namespace RetroGames.Services.Servicos;
public sealed class PlataformaService(IRepositorioSet repositorios) : IPlataformaService
{
    public async Task<IReadOnlyList<PlataformaServiceModel>> ListarAsync(CancellationToken ct=default)=>(await repositorios.Plataformas.ListarAsync(ct)).Select(PlataformaServiceModel.De).ToList();
    public async Task<PlataformaServiceModel> ObterAsync(int id,CancellationToken ct=default)=>PlataformaServiceModel.De(await repositorios.Plataformas.ObterAsync(id,ct)??throw BusinessErrors.PlataformaNaoEncontrada());
    public async Task<PlataformaServiceModel> InserirAsync(Plataforma item,CancellationToken ct=default)
    {
        if(await repositorios.Plataformas.ExisteNomeAsync(item.Nome,ct:ct))throw BusinessErrors.PlataformaNomeDuplicado();
        try{item.Id=await repositorios.Plataformas.InserirAsync(item,ct);}catch(RegistroDuplicadoException){throw BusinessErrors.PlataformaNomeDuplicado();}
        return PlataformaServiceModel.De(item);
    }
    public async Task AtualizarAsync(Plataforma item,CancellationToken ct=default)
    {
        if(await repositorios.Plataformas.ObterAsync(item.Id,ct) is null)throw BusinessErrors.PlataformaNaoEncontrada();
        if(await repositorios.Plataformas.ExisteNomeAsync(item.Nome,item.Id,ct))throw BusinessErrors.PlataformaNomeDuplicado();
        try{await repositorios.Plataformas.AtualizarAsync(item,ct);}catch(RegistroDuplicadoException){throw BusinessErrors.PlataformaNomeDuplicado();}
    }
    public async Task ExcluirAsync(int id,CancellationToken ct=default)
    {
        if(await repositorios.Plataformas.ObterAsync(id,ct) is null)throw BusinessErrors.PlataformaNaoEncontrada();
        if(await repositorios.Plataformas.PossuiJogosAsync(id,ct))throw BusinessErrors.PlataformaPossuiJogos();
        await repositorios.Plataformas.ExcluirAsync(id,ct);
    }
}
