using System.Security.Claims;
using RetroGames.Services.Interfaces;

namespace RetroGames.WebApi.Servicos;

public sealed class UsuarioAtualService(IHttpContextAccessor accessor) : IUsuarioAtualService
{
    private ClaimsPrincipal? Usuario => accessor.HttpContext?.User;
    public int? Id => int.TryParse(Usuario?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    public string? Nome => Usuario?.FindFirstValue(ClaimTypes.Name);
    public string? Email => Usuario?.FindFirstValue(ClaimTypes.Email);
    public string? Perfil => Usuario?.FindFirstValue(ClaimTypes.Role);
    public bool EhAdministrador => Usuario?.IsInRole("Administrador") == true;
}
