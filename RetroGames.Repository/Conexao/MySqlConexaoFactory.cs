using MySqlConnector;
namespace RetroGames.Repository.Conexao;
public sealed class MySqlConexaoFactory(string connectionString)
{
    public MySqlConnection Criar() => new(connectionString);
}
