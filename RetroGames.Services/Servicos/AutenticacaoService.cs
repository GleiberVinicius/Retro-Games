using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Excecoes;
using RetroGames.Services.Interfaces;
namespace RetroGames.Services.Servicos;
public sealed class AutenticacaoService(IRepositorioSet repositorios) : IAutenticacaoService
{
    private readonly PasswordHasher<Usuario> _hasher = new();
    public async Task InicializarAdministradorAsync(string nome,string email,string senha,CancellationToken ct=default)
    {
        if(await repositorios.Usuarios.ObterPorEmailAsync(email,ct)is not null)return;
        var usuario=new Usuario{Nome=nome,Email=email,Ativo=true,DataCadastro=DateTime.UtcNow,IdPerfil=1};usuario.SenhaHash=_hasher.HashPassword(usuario,senha);
        try{await repositorios.Usuarios.InserirAsync(usuario,ct);}catch(RetroGames.Model.Exceptions.RegistroDuplicadoException){ /* outro processo criou a conta ao mesmo tempo */ }
    }
    public async Task<Usuario> AutenticarAsync(string email,string senha,CancellationToken ct=default)
    {
        var usuario=await repositorios.Usuarios.ObterPorEmailAsync(email,ct);
        if(usuario is null||!usuario.Ativo||_hasher.VerifyHashedPassword(usuario,usuario.SenhaHash,senha)==PasswordVerificationResult.Failed)throw BusinessErrors.CredenciaisInvalidas();
        return usuario;
    }
    public async Task<string> ObterPerfilAsync(int idPerfil,CancellationToken ct=default)=>await repositorios.Perfis.ObterDescricaoAsync(idPerfil,ct)??string.Empty;
}
