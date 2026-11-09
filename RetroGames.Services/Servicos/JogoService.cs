using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Excecoes;
using RetroGames.Services.Interfaces;
using RetroGames.Services.Modelos;
namespace RetroGames.Services.Servicos;
public sealed class JogoService(IRepositorioSet repositorios) : IJogoService
{
    public async Task<PaginaServiceModel<JogoDetalheServiceModel>> ListarAsync(string? busca,int? idPlataforma,int? idGenero,int pagina,int tamanhoPagina,bool incluirInativos,CancellationToken ct=default)
    {
        IReadOnlyCollection<int>? ids=null;
        if(idGenero.HasValue) ids=(await repositorios.JogosGeneros.ListarTodosAsync(ct)).Where(x=>x.IdGenero==idGenero.Value).Select(x=>x.IdJogo).Distinct().ToArray();
        var itens=await repositorios.Jogos.ListarAsync(busca,idPlataforma,incluirInativos,pagina,tamanhoPagina,ids,ct);
        var total=await repositorios.Jogos.ContarAsync(busca,idPlataforma,incluirInativos,ids,ct);
        var plataformas=await repositorios.Plataformas.ListarAsync(ct); var condicoes=await repositorios.Condicoes.ListarAsync(ct); var generos=await repositorios.Generos.ListarAsync(ct); var relacoes=await repositorios.JogosGeneros.ListarPorJogosAsync(itens.Select(j=>j.Id).ToArray(),ct);
        var modelos=itens.Select(j=>Montar(j,plataformas,condicoes,generos,relacoes)).ToArray();
        return new(modelos,pagina,tamanhoPagina,total);
    }
    public async Task<JogoDetalheServiceModel> ObterAsync(int id,bool incluirInativo,CancellationToken ct=default)
    {
        var jogo=await repositorios.Jogos.ObterAsync(id,incluirInativo,ct)??throw BusinessErrors.JogoNaoEncontrado();
        var plataforma=await repositorios.Plataformas.ObterAsync(jogo.IdPlataforma,ct); var condicao=(await repositorios.Condicoes.ListarAsync(ct)).FirstOrDefault(x=>x.Id==jogo.IdCondicaoJogo);
        var ligacoes=(await repositorios.JogosGeneros.ListarPorJogosAsync([id],ct)).Select(x=>x.IdGenero).ToArray(); var generos=await repositorios.Generos.ListarPorIdsAsync(ligacoes,ct);
        return new(jogo.Id,jogo.Nome,jogo.Descricao,jogo.AnoLancamento,jogo.Preco,jogo.Estoque,jogo.Ativo,jogo.DataCadastro,jogo.IdPlataforma,plataforma?.Nome??string.Empty,jogo.IdCondicaoJogo,condicao?.Descricao??string.Empty,generos);
    }
    public async Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo,IReadOnlyCollection<int> idsGeneros,CancellationToken ct=default)
    {
        jogo.Nome=jogo.Nome.Trim();
        await ValidarReferenciasAsync(jogo,idsGeneros,ct); jogo.Ativo=true;
        var opcoes=new TransactionOptions{IsolationLevel=IsolationLevel.ReadCommitted,Timeout=TransactionManager.DefaultTimeout};
        using(var escopo=new TransactionScope(TransactionScopeOption.Required,opcoes,TransactionScopeAsyncFlowOption.Enabled))
        { jogo.Id=await repositorios.Jogos.InserirAsync(jogo,ct); await repositorios.JogosGeneros.InserirAsync(jogo.Id,idsGeneros,ct); escopo.Complete(); }
        return await ObterAsync(jogo.Id,true,ct);
    }
    public async Task AtualizarAsync(Jogo jogo,IReadOnlyCollection<int> idsGeneros,CancellationToken ct=default)
    {
        jogo.Nome=jogo.Nome.Trim();
        if(await repositorios.Jogos.ObterAsync(jogo.Id,true,ct) is null)throw BusinessErrors.JogoNaoEncontrado(); await ValidarReferenciasAsync(jogo,idsGeneros,ct);
        var opcoes=new TransactionOptions{IsolationLevel=IsolationLevel.ReadCommitted,Timeout=TransactionManager.DefaultTimeout};
        using(var escopo=new TransactionScope(TransactionScopeOption.Required,opcoes,TransactionScopeAsyncFlowOption.Enabled))
        { await repositorios.Jogos.AtualizarAsync(jogo,ct); await repositorios.JogosGeneros.RemoverPorJogoAsync(jogo.Id,ct); await repositorios.JogosGeneros.InserirAsync(jogo.Id,idsGeneros,ct); escopo.Complete(); }
    }
    public async Task DesativarAsync(int id,CancellationToken ct=default){if(await repositorios.Jogos.ObterAsync(id,true,ct)is null)throw BusinessErrors.JogoNaoEncontrado();await repositorios.Jogos.DesativarAsync(id,ct);}
    private async Task ValidarReferenciasAsync(Jogo jogo,IReadOnlyCollection<int> ids,CancellationToken ct)
    {
        if(await repositorios.Plataformas.ObterAsync(jogo.IdPlataforma,ct)is null)throw BusinessErrors.PlataformaInexistente();
        if(!await repositorios.Condicoes.ExisteAsync(jogo.IdCondicaoJogo,ct))throw BusinessErrors.CondicaoInexistente();
        if(ids.Count==0|| (await repositorios.Generos.ListarPorIdsAsync(ids.Distinct().ToArray(),ct)).Count!=ids.Distinct().Count())throw BusinessErrors.GeneroInexistente();
    }
    private static JogoDetalheServiceModel Montar(Jogo j,IReadOnlyList<Plataforma> p,IReadOnlyList<CondicaoJogo> c,IReadOnlyList<Genero> g,IReadOnlyList<JogoGenero> jg)
    { var plataforma=p.FirstOrDefault(x=>x.Id==j.IdPlataforma);var condicao=c.FirstOrDefault(x=>x.Id==j.IdCondicaoJogo);var ids=jg.Where(x=>x.IdJogo==j.Id).Select(x=>x.IdGenero).ToHashSet();var generos=g.Where(x=>ids.Contains(x.Id)).ToArray();return new(j.Id,j.Nome,j.Descricao,j.AnoLancamento,j.Preco,j.Estoque,j.Ativo,j.DataCadastro,j.IdPlataforma,plataforma?.Nome??string.Empty,j.IdCondicaoJogo,condicao?.Descricao??string.Empty,generos); }
}
