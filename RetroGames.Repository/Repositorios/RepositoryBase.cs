using MySqlConnector;
using RetroGames.Model.Exceptions;
using RetroGames.Repository.Conexao;

namespace RetroGames.Repository.Repositorios;

public abstract class RepositoryBase(RepositoryConfig config)
{
    protected async Task<MySqlConnection> AbrirConexaoAsync(CancellationToken ct)
    {
        var conexao = new MySqlConnection(config.ConnectionString);

        try
        {
            await conexao.OpenAsync(ct);
            return conexao;
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
    }

    protected MySqlCommand CriarComando(MySqlConnection conexao, string textoSql)
        => new(textoSql, conexao);

    protected void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
        => comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);

    protected async Task<int> ExecutarGravacaoAsync(MySqlCommand comando, CancellationToken ct)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync(ct);
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new RegistroDuplicadoException("Registro duplicado.");
        }
    }
}
