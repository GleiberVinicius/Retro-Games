using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Model;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Mapeadores;
namespace RetroGames.WebApi.Controllers;
[Route("api/[controller]")]
public sealed class PlataformaController(IPlataformaService servico) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Listar(CancellationToken ct)=>Ok(EnvelopeResposta<IReadOnlyList<ListarPlataformaResponseApiModel>>.Sucesso((await servico.ListarAsync(ct)).Select(x=>x.ParaResponse()).ToArray()));
    [HttpGet("{id:int}")] public async Task<IActionResult> Obter(int id,CancellationToken ct){var plataforma=await servico.ObterAsync(id,ct);return Ok(EnvelopeResposta<ListarPlataformaResponseApiModel>.Sucesso(plataforma.ParaResponse()));}
    [HttpPost,Authorize(Policy="SomenteAdministrador")] public async Task<IActionResult> Inserir([FromBody]PlataformaEntradaApiModel modelo,CancellationToken ct){var plataforma=await servico.InserirAsync(new(){Nome=modelo.Nome.Trim(),Fabricante=modelo.Fabricante.Trim(),AnoLancamento=modelo.AnoLancamento},ct);var data=plataforma.ParaResponse();return CreatedAtAction(nameof(Obter),new{id=plataforma.Id},EnvelopeResposta<ListarPlataformaResponseApiModel>.Sucesso(data));}
    [HttpPut("{id:int}"),Authorize(Policy="SomenteAdministrador")] public async Task<IActionResult> Atualizar(int id,[FromBody]PlataformaEntradaApiModel modelo,CancellationToken ct){await servico.AtualizarAsync(new(){Id=id,Nome=modelo.Nome.Trim(),Fabricante=modelo.Fabricante.Trim(),AnoLancamento=modelo.AnoLancamento},ct);return Ok(EnvelopeResposta<object>.Sucesso(new{}));}
    [HttpDelete("{id:int}"),Authorize(Policy="SomenteAdministrador")] public async Task<IActionResult> Excluir(int id,CancellationToken ct){await servico.ExcluirAsync(id,ct);return NoContent();}
}
