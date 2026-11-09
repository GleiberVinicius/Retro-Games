using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.ApiModels;
namespace RetroGames.WebApi.Controllers;
[Route("api/[controller]")]
public sealed class TabelaTipoController(ICondicaoJogoService servico):ControllerBase
{ [HttpGet("condicoes-jogo")] public async Task<IActionResult> Condicoes(CancellationToken ct)=>Ok(EnvelopeResposta<IReadOnlyList<CondicaoListaApiModel>>.Sucesso((await servico.ListarAsync(ct)).Select(c=>new CondicaoListaApiModel(c.Id,c.Descricao)).ToArray())); }
