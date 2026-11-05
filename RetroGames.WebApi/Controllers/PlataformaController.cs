using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Model;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.ApiModels;
namespace RetroGames.WebApi.Controllers;
[Route("api/[controller]")]
public sealed class PlataformaController(IPlataformaService servico) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Listar(CancellationToken ct)=>Ok(EnvelopeResposta<IReadOnlyList<PlataformaApiModel>>.Sucesso((await servico.ListarAsync(ct)).Select(x=>new PlataformaApiModel(x.Id,x.Nome,x.Fabricante,x.AnoLancamento)).ToArray()));
    [HttpGet("{id:int}")] public async Task<IActionResult> Obter(int id,CancellationToken ct){var x=await servico.ObterAsync(id,ct);return Ok(EnvelopeResposta<PlataformaApiModel>.Sucesso(new(x.Id,x.Nome,x.Fabricante,x.AnoLancamento)));}
    [HttpPost,Authorize(Policy="SomenteAdministrador")] public async Task<IActionResult> Inserir([FromBody]PlataformaEntradaApiModel modelo,CancellationToken ct){var x=await servico.InserirAsync(new(){Nome=modelo.Nome.Trim(),Fabricante=modelo.Fabricante.Trim(),AnoLancamento=modelo.AnoLancamento},ct);var data=new PlataformaApiModel(x.Id,x.Nome,x.Fabricante,x.AnoLancamento);return CreatedAtAction(nameof(Obter),new{id=x.Id},EnvelopeResposta<PlataformaApiModel>.Sucesso(data));}
    [HttpPut("{id:int}"),Authorize(Policy="SomenteAdministrador")] public async Task<IActionResult> Atualizar(int id,[FromBody]PlataformaEntradaApiModel modelo,CancellationToken ct){await servico.AtualizarAsync(new(){Id=id,Nome=modelo.Nome.Trim(),Fabricante=modelo.Fabricante.Trim(),AnoLancamento=modelo.AnoLancamento},ct);return Ok(EnvelopeResposta<object>.Sucesso(new{}));}
    [HttpDelete("{id:int}"),Authorize(Policy="SomenteAdministrador")] public async Task<IActionResult> Excluir(int id,CancellationToken ct){await servico.ExcluirAsync(id,ct);return NoContent();}
}
