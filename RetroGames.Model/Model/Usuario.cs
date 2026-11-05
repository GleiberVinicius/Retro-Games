namespace RetroGames.Model.Model;
public sealed class Usuario
{
    public int Id { get; set; } // co_seq_usuario
    public string Nome { get; set; } = string.Empty; // no_usuario
    public string Email { get; set; } = string.Empty; // ds_email
    public string SenhaHash { get; set; } = string.Empty; // ds_senha_hash
    public bool Ativo { get; set; } // st_ativo
    public DateTime DataCadastro { get; set; } // dt_cadastro
    public int IdPerfil { get; set; } // co_perfil
}
