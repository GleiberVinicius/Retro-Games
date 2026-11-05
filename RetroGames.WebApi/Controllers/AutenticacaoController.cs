using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.ApiModels;
namespace RetroGames.WebApi.Controllers;
[Route("api/[controller]")]
public sealed class AutenticacaoController(IAutenticacaoService servico,IAntiforgery antiforgery):ControllerBase
{
    [HttpGet("csrf")] [AllowAnonymous] public IActionResult Csrf(){var tokens=antiforgery.GetAndStoreTokens(HttpContext);Response.Cookies.Append("XSRF-TOKEN",tokens.RequestToken!,new CookieOptions{HttpOnly=false,SameSite=SameSiteMode.Strict,Secure=Request.IsHttps,IsEssential=true});return Ok(EnvelopeResposta<object>.Sucesso(new{}));}
    [HttpPost("login"),AllowAnonymous] public async Task<IActionResult> Login([FromBody]LoginApiModel modelo,CancellationToken ct){var usuario=await servico.AutenticarAsync(modelo.Email,modelo.Senha,ct);var perfil=await servico.ObterPerfilAsync(usuario.IdPerfil,ct);var claims=new[]{new Claim(ClaimTypes.NameIdentifier,usuario.Id.ToString()),new Claim(ClaimTypes.Name,usuario.Nome),new Claim(ClaimTypes.Email,usuario.Email),new Claim(ClaimTypes.Role,perfil)};await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme)));Csrf();return Ok(EnvelopeResposta<UsuarioAutenticadoApiModel>.Sucesso(new(usuario.Id,usuario.Nome,usuario.Email,perfil)));}
    [HttpPost("logout"),Authorize(Policy="SomenteAdministrador")] public async Task<IActionResult> Logout(){await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);Csrf();return Ok(EnvelopeResposta<object>.Sucesso(new{}));}
    [HttpGet("eu"),Authorize(Policy="SomenteAdministrador")] public IActionResult Eu()=>Ok(EnvelopeResposta<UsuarioAutenticadoApiModel>.Sucesso(new(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),User.Identity?.Name??string.Empty,User.FindFirstValue(ClaimTypes.Email)??string.Empty,User.FindFirstValue(ClaimTypes.Role)??string.Empty)));
}
