namespace RetroGames.WebApi.ApiModels.ApiModels;
public sealed class LoginApiModel { public string Email { get; set; } = string.Empty; public string Senha { get; set; } = string.Empty; }
public sealed record UsuarioAutenticadoApiModel(int Id, string Nome, string Email, string Perfil);
