using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Mapeadores;

namespace RetroGames.WebApi.Controllers;

[Route("api/[controller]")]
public sealed class CondicaoJogoController(ICondicaoJogoService servico) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
        => Ok(EnvelopeResposta<IReadOnlyList<ListarCondicaoJogoResponseApiModel>>.Sucesso(
            (await servico.ListarAsync(ct)).Select(condicao => condicao.ParaResponse()).ToArray()));
}
