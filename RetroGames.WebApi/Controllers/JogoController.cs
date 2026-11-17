using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Mapeadores;
namespace RetroGames.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public sealed class JogoController(IJogoService servico, IUsuarioAtualService usuarioAtual):ControllerBase
{
    [HttpGet,AllowAnonymous] public async Task<IActionResult> Listar([FromQuery]ListarJogoRequestApiModel consulta,CancellationToken ct)
    {
        var filtro=consulta.ParaFiltro();
        var resultado=await servico.ListarAsync(filtro.Busca,filtro.IdPlataforma,filtro.IdGenero,filtro.Pagina,filtro.TamanhoPagina,incluirInativos:consulta.IncluirInativos && usuarioAtual.EhAdministrador,ct);
        var data=new PaginaResponseApiModel<ListarJogoResponseApiModel>(resultado.Itens.Select(jogo=>jogo.ParaResponse()).ToArray(),resultado.Pagina,resultado.TamanhoPagina,resultado.TotalItens,(int)Math.Ceiling(resultado.TotalItens/(double)resultado.TamanhoPagina));
        return Ok(EnvelopeResposta<PaginaResponseApiModel<ListarJogoResponseApiModel>>.Sucesso(data));
    }
    [HttpGet("{id:int}"),AllowAnonymous] public async Task<IActionResult> Obter(int id,CancellationToken ct){var jogo=await servico.ObterAsync(id,usuarioAtual.EhAdministrador,ct);return Ok(EnvelopeResposta<ListarPorIdJogoResponseApiModel>.Sucesso(jogo.ParaResponseDetalhada()));}
    [HttpPost,Authorize(Policy="Politicas.Administrador")] public async Task<IActionResult> Inserir([FromBody]InserirJogoRequestApiModel modelo,CancellationToken ct){var jogo=await servico.InserirAsync(modelo.ParaModel(),modelo.IdsGeneros,ct);return CreatedAtAction(nameof(Obter),new{id=jogo.Id},EnvelopeResposta<ListarPorIdJogoResponseApiModel>.Sucesso(jogo.ParaResponseDetalhada()));}
    [HttpPut("{id:int}"),Authorize(Policy="Politicas.Administrador")] public async Task<IActionResult> Alterar(int id,[FromBody]AlterarJogoRequestApiModel modelo,CancellationToken ct){await servico.AtualizarAsync(modelo.ParaModel(id),modelo.IdsGeneros,ct);return Ok(EnvelopeResposta<object>.Sucesso(new{}));}
    [HttpDelete("{id:int}"),Authorize(Policy="Politicas.Administrador")] public async Task<IActionResult> Excluir(int id,CancellationToken ct){await servico.DesativarAsync(id,ct);return NoContent();}
}
