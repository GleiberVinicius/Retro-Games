using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Mapeadores;
namespace RetroGames.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public sealed class PlataformaController(IPlataformaService servico) : ControllerBase
{
    [HttpGet,AllowAnonymous] public async Task<IActionResult> Listar(CancellationToken ct)=>Ok(EnvelopeResposta<IReadOnlyList<ListarPlataformaResponseApiModel>>.Sucesso((await servico.ListarAsync(ct)).Select(x=>x.ParaResponse()).ToArray()));
    [HttpGet("{id:int}"),AllowAnonymous] public async Task<IActionResult> Obter(int id,CancellationToken ct){var plataforma=await servico.ObterAsync(id,ct);return Ok(EnvelopeResposta<ListarPlataformaResponseApiModel>.Sucesso(plataforma.ParaResponse()));}
    [HttpPost,Authorize(Policy="Politicas.Administrador")] public async Task<IActionResult> Inserir([FromBody]InserirPlataformaRequestApiModel modelo,CancellationToken ct){var plataforma=await servico.InserirAsync(modelo.ParaModel(),ct);var data=plataforma.ParaResponse();return CreatedAtAction(nameof(Obter),new{id=plataforma.Id},EnvelopeResposta<ListarPlataformaResponseApiModel>.Sucesso(data));}
    [HttpPut("{id:int}"),Authorize(Policy="Politicas.Administrador")] public async Task<IActionResult> Alterar(int id,[FromBody]AlterarPlataformaRequestApiModel modelo,CancellationToken ct){await servico.AtualizarAsync(modelo.ParaModel(id),ct);return Ok(EnvelopeResposta<object>.Sucesso(new{}));}
    [HttpDelete("{id:int}"),Authorize(Policy="Politicas.Administrador")] public async Task<IActionResult> Excluir(int id,CancellationToken ct){await servico.ExcluirAsync(id,ct);return Ok(EnvelopeResposta<object>.Sucesso(new{}));}
}
