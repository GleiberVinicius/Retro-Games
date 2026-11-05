using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Model;
using RetroGames.Services.Interfaces;
using RetroGames.Services.Modelos;
using RetroGames.WebApi.ApiModels.ApiModels;
namespace RetroGames.WebApi.Controllers;
[Route("api/[controller]")]
public sealed class JogoController(IJogoService servico):ControllerBase
{
    [HttpGet] public async Task<IActionResult> Listar([FromQuery]JogoConsultaApiModel consulta,CancellationToken ct)
    {var admin=User.IsInRole("Administrador");var resultado=await servico.ListarAsync(consulta.Busca,consulta.IdPlataforma,consulta.IdGenero,consulta.Pagina,consulta.TamanhoPagina,consulta.IncluirInativos&&admin,ct);var data=new PaginaApiModel<JogoApiModel>(resultado.Itens.Select(Mapear).ToArray(),resultado.Pagina,resultado.TamanhoPagina,resultado.TotalItens,(int)Math.Ceiling(resultado.TotalItens/(double)resultado.TamanhoPagina));return Ok(EnvelopeResposta<PaginaApiModel<JogoApiModel>>.Sucesso(data));}
    [HttpGet("{id:int}")] public async Task<IActionResult> Obter(int id,CancellationToken ct){var x=await servico.ObterAsync(id,User.IsInRole("Administrador"),ct);return Ok(EnvelopeResposta<JogoApiModel>.Sucesso(Mapear(x)));}
    [HttpPost,Authorize(Policy="SomenteAdministrador")] public async Task<IActionResult> Inserir([FromBody]JogoEntradaApiModel modelo,CancellationToken ct){var x=await servico.InserirAsync(Criar(modelo),modelo.IdsGeneros,ct);return CreatedAtAction(nameof(Obter),new{id=x.Id},EnvelopeResposta<JogoApiModel>.Sucesso(Mapear(x)));}
    [HttpPut("{id:int}"),Authorize(Policy="SomenteAdministrador")] public async Task<IActionResult> Atualizar(int id,[FromBody]JogoEntradaApiModel modelo,CancellationToken ct){var jogo=Criar(modelo);jogo.Id=id;await servico.AtualizarAsync(jogo,modelo.IdsGeneros,ct);return Ok(EnvelopeResposta<object>.Sucesso(new{}));}
    [HttpDelete("{id:int}"),Authorize(Policy="SomenteAdministrador")] public async Task<IActionResult> Excluir(int id,CancellationToken ct){await servico.DesativarAsync(id,ct);return NoContent();}
    private static Jogo Criar(JogoEntradaApiModel m)=>new(){Nome=m.Nome.Trim(),Descricao=m.Descricao,AnoLancamento=m.AnoLancamento,Preco=m.Preco,Estoque=m.Estoque,IdPlataforma=m.IdPlataforma,IdCondicaoJogo=m.IdCondicaoJogo};
    private static JogoApiModel Mapear(JogoDetalheServiceModel j)=>new(j.Id,j.Nome,j.Descricao,j.AnoLancamento,j.Preco,j.Estoque,j.Ativo,j.DataCadastro,j.IdPlataforma,j.Plataforma,j.IdCondicaoJogo,j.Condicao,j.Generos.Select(g=>new GeneroApiModel(g.Id,g.Nome)).ToArray());
}
