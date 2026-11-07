namespace RetroGames.Repository.Conexao;

public sealed class RepositoryConfig(string connectionString)
{
    public string ConnectionString { get; } = connectionString;
}
