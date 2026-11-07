using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Conexao;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositorios;

public sealed class RepositorioSet(RepositoryConfig config) : IRepositorioSet
{
    public IPlataformaRepository Plataformas { get; } = new PlataformaRepository(config);
    public IJogoRepository Jogos { get; } = new JogoRepository(config);
    public IGeneroRepository Generos { get; } = new GeneroRepository(config);
    public ICondicaoJogoRepository Condicoes { get; } = new CondicaoJogoRepository(config);
    public IUsuarioRepository Usuarios { get; } = new UsuarioRepository(config);
    public IPerfilRepository Perfis { get; } = new PerfilRepository(config);
    public IJogoGeneroRepository JogosGeneros { get; } = new JogoGeneroRepository(config);
}

internal sealed class PlataformaRepository(RepositoryConfig config)
    : RepositoryBase(config), IPlataformaRepository
{
    public async Task<IReadOnlyList<Plataforma>> ListarAsync(CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma ORDER BY no_plataforma");
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        var itens = new List<Plataforma>();
        while (await rd.ReadAsync(ct)) itens.Add(Mapear(rd));
        return itens;
    }

    public async Task<Plataforma?> ObterAsync(int id, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma WHERE co_seq_plataforma = @id");
        AdicionarParametro(cmd, "@id", id);
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        return await rd.ReadAsync(ct) ? Mapear(rd) : null;
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? ignorarId = null, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        var sql = "SELECT COUNT(*) FROM tb_plataforma WHERE no_plataforma = @nome" + (ignorarId.HasValue ? " AND co_seq_plataforma <> @id" : "");
        await using var cmd = CriarComando(cn, sql);
        AdicionarParametro(cmd, "@nome", nome);
        if (ignorarId.HasValue) AdicionarParametro(cmd, "@id", ignorarId.Value);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)) > 0;
    }

    public async Task<bool> PossuiJogosAsync(int idPlataforma, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "SELECT COUNT(*) FROM tb_jogo WHERE co_plataforma=@id");
        AdicionarParametro(cmd, "@id", idPlataforma);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)) > 0;
    }

    public async Task<int> InserirAsync(Plataforma item, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) VALUES (@nome, @fabricante, @ano)");
        AdicionarParametro(cmd, "@nome", item.Nome);
        AdicionarParametro(cmd, "@fabricante", item.Fabricante);
        AdicionarParametro(cmd, "@ano", item.AnoLancamento);
        await ExecutarGravacaoAsync(cmd, ct);
        return checked((int)cmd.LastInsertedId);
    }

    public async Task AtualizarAsync(Plataforma item, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "UPDATE tb_plataforma SET no_plataforma=@nome, no_fabricante=@fabricante, nu_ano_lancamento=@ano WHERE co_seq_plataforma=@id");
        AdicionarParametro(cmd, "@nome", item.Nome);
        AdicionarParametro(cmd, "@fabricante", item.Fabricante);
        AdicionarParametro(cmd, "@ano", item.AnoLancamento);
        AdicionarParametro(cmd, "@id", item.Id);
        await ExecutarGravacaoAsync(cmd, ct);
    }

    public async Task ExcluirAsync(int id, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "DELETE FROM tb_plataforma WHERE co_seq_plataforma=@id");
        AdicionarParametro(cmd, "@id", id);
        await ExecutarGravacaoAsync(cmd, ct);
    }

    private static Plataforma Mapear(MySqlDataReader rd) => new()
    {
        Id = rd.GetInt32("co_seq_plataforma"),
        Nome = rd.GetString("no_plataforma"),
        Fabricante = rd.GetString("no_fabricante"),
        AnoLancamento = rd.GetInt16("nu_ano_lancamento"),
    };
}

internal sealed class JogoRepository(RepositoryConfig config)
    : RepositoryBase(config), IJogoRepository
{
    private static string Filtros(string? busca, int? plataforma, bool inativos, IReadOnlyCollection<int>? ids)
    {
        var partes = new List<string>();
        if (!inativos) partes.Add("st_ativo=1");
        if (plataforma.HasValue) partes.Add("co_plataforma=@plataforma");
        if (!string.IsNullOrWhiteSpace(busca)) partes.Add("no_jogo LIKE @busca");
        if (ids is { Count: > 0 }) partes.Add("co_seq_jogo IN (" + string.Join(",", ids.Select((_, i) => "@j" + i)) + ")");
        else if (ids is { Count: 0 }) partes.Add("1=0");
        return partes.Count == 0 ? "" : " WHERE " + string.Join(" AND ", partes);
    }

    private void Parametros(MySqlCommand cmd, string? busca, int? plataforma, IReadOnlyCollection<int>? ids)
    {
        if (plataforma.HasValue) AdicionarParametro(cmd, "@plataforma", plataforma.Value);
        if (!string.IsNullOrWhiteSpace(busca)) AdicionarParametro(cmd, "@busca", "%" + busca.Trim() + "%");
        if (ids is not null)
            for (var i = 0; i < ids.Count; i++) AdicionarParametro(cmd, "@j" + i, ids.ElementAt(i));
    }

    public async Task<IReadOnlyList<Jogo>> ListarAsync(string? busca, int? idPlataforma, bool incluirInativos, int pagina, int tamanhoPagina, IReadOnlyCollection<int>? ids, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        var sql = "SELECT co_seq_jogo,no_jogo,ds_jogo,nu_ano_lancamento,vl_preco,qt_estoque,st_ativo,dt_cadastro,co_plataforma,co_condicao_jogo FROM tb_jogo" + Filtros(busca, idPlataforma, incluirInativos, ids) + " ORDER BY no_jogo LIMIT @limite OFFSET @offset";
        await using var cmd = CriarComando(cn, sql);
        Parametros(cmd, busca, idPlataforma, ids);
        AdicionarParametro(cmd, "@limite", tamanhoPagina);
        AdicionarParametro(cmd, "@offset", (pagina - 1) * tamanhoPagina);
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        var itens = new List<Jogo>();
        while (await rd.ReadAsync(ct)) itens.Add(Mapear(rd));
        return itens;
    }

    public async Task<int> ContarAsync(string? busca, int? idPlataforma, bool incluirInativos, IReadOnlyCollection<int>? ids, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "SELECT COUNT(*) FROM tb_jogo" + Filtros(busca, idPlataforma, incluirInativos, ids));
        Parametros(cmd, busca, idPlataforma, ids);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<Jogo?> ObterAsync(int id, bool incluirInativo, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        var sql = "SELECT co_seq_jogo,no_jogo,ds_jogo,nu_ano_lancamento,vl_preco,qt_estoque,st_ativo,dt_cadastro,co_plataforma,co_condicao_jogo FROM tb_jogo WHERE co_seq_jogo=@id" + (incluirInativo ? "" : " AND st_ativo=1");
        await using var cmd = CriarComando(cn, sql);
        AdicionarParametro(cmd, "@id", id);
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        return await rd.ReadAsync(ct) ? Mapear(rd) : null;
    }

    public async Task<int> InserirAsync(Jogo jogo, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "INSERT INTO tb_jogo(no_jogo,ds_jogo,nu_ano_lancamento,vl_preco,qt_estoque,st_ativo,dt_cadastro,co_plataforma,co_condicao_jogo) VALUES(@nome,@descricao,@ano,@preco,@estoque,1,UTC_TIMESTAMP(),@plataforma,@condicao)");
        ParametrosJogo(cmd, jogo);
        await ExecutarGravacaoAsync(cmd, ct);
        return checked((int)cmd.LastInsertedId);
    }

    public async Task AtualizarAsync(Jogo jogo, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "UPDATE tb_jogo SET no_jogo=@nome,ds_jogo=@descricao,nu_ano_lancamento=@ano,vl_preco=@preco,qt_estoque=@estoque,co_plataforma=@plataforma,co_condicao_jogo=@condicao WHERE co_seq_jogo=@id");
        ParametrosJogo(cmd, jogo);
        AdicionarParametro(cmd, "@id", jogo.Id);
        await ExecutarGravacaoAsync(cmd, ct);
    }

    public async Task DesativarAsync(int id, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "UPDATE tb_jogo SET st_ativo=0 WHERE co_seq_jogo=@id");
        AdicionarParametro(cmd, "@id", id);
        await ExecutarGravacaoAsync(cmd, ct);
    }

    private void ParametrosJogo(MySqlCommand cmd, Jogo jogo)
    {
        AdicionarParametro(cmd, "@nome", jogo.Nome);
        AdicionarParametro(cmd, "@descricao", jogo.Descricao);
        AdicionarParametro(cmd, "@ano", jogo.AnoLancamento);
        AdicionarParametro(cmd, "@preco", jogo.Preco);
        AdicionarParametro(cmd, "@estoque", jogo.Estoque);
        AdicionarParametro(cmd, "@plataforma", jogo.IdPlataforma);
        AdicionarParametro(cmd, "@condicao", jogo.IdCondicaoJogo);
    }

    private static Jogo Mapear(MySqlDataReader r) => new()
    {
        Id = r.GetInt32("co_seq_jogo"),
        Nome = r.GetString("no_jogo"),
        Descricao = r.IsDBNull(r.GetOrdinal("ds_jogo")) ? null : r.GetString("ds_jogo"),
        AnoLancamento = r.GetInt16("nu_ano_lancamento"),
        Preco = r.GetDecimal("vl_preco"),
        Estoque = r.GetInt32("qt_estoque"),
        Ativo = r.GetBoolean("st_ativo"),
        DataCadastro = DateTime.SpecifyKind(r.GetDateTime("dt_cadastro"), DateTimeKind.Utc),
        IdPlataforma = r.GetInt32("co_plataforma"),
        IdCondicaoJogo = r.GetInt32("co_condicao_jogo"),
    };
}

internal sealed class GeneroRepository(RepositoryConfig config)
    : RepositoryBase(config), IGeneroRepository
{
    public Task<IReadOnlyList<Genero>> ListarAsync(CancellationToken ct = default)
        => ConsultarAsync("SELECT co_seq_genero,no_genero FROM tb_genero ORDER BY no_genero", null, ct);

    public async Task<IReadOnlyList<Genero>> ListarPorIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct = default)
    {
        if (ids.Count == 0) return [];
        await using var cn = await AbrirConexaoAsync(ct);
        var parametros = string.Join(",", ids.Select((_, i) => "@i" + i));
        await using var cmd = CriarComando(cn, "SELECT co_seq_genero,no_genero FROM tb_genero WHERE co_seq_genero IN (" + parametros + ")");
        for (var i = 0; i < ids.Count; i++) AdicionarParametro(cmd, "@i" + i, ids.ElementAt(i));
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        var itens = new List<Genero>();
        while (await rd.ReadAsync(ct)) itens.Add(Mapear(rd));
        return itens;
    }

    private async Task<IReadOnlyList<Genero>> ConsultarAsync(string sql, int? id, CancellationToken ct)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, sql);
        if (id.HasValue) AdicionarParametro(cmd, "@id", id.Value);
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        var itens = new List<Genero>();
        while (await rd.ReadAsync(ct)) itens.Add(Mapear(rd));
        return itens;
    }

    private static Genero Mapear(MySqlDataReader rd) => new()
    {
        Id = rd.GetInt32(0),
        Nome = rd.GetString(1),
    };
}

internal sealed class CondicaoJogoRepository(RepositoryConfig config)
    : RepositoryBase(config), ICondicaoJogoRepository
{
    public async Task<IReadOnlyList<CondicaoJogo>> ListarAsync(CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "SELECT co_condicao_jogo,ds_condicao_jogo FROM tb_condicao_jogo ORDER BY co_condicao_jogo");
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        var itens = new List<CondicaoJogo>();
        while (await rd.ReadAsync(ct)) itens.Add(new() { Id = rd.GetInt32(0), Descricao = rd.GetString(1) });
        return itens;
    }

    public async Task<bool> ExisteAsync(int id, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "SELECT COUNT(*) FROM tb_condicao_jogo WHERE co_condicao_jogo=@id");
        AdicionarParametro(cmd, "@id", id);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)) > 0;
    }
}

internal sealed class JogoGeneroRepository(RepositoryConfig config)
    : RepositoryBase(config), IJogoGeneroRepository
{
    public async Task<IReadOnlyList<JogoGenero>> ListarTodosAsync(CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "SELECT co_jogo,co_genero FROM tb_jogo_genero");
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        var itens = new List<JogoGenero>();
        while (await rd.ReadAsync(ct)) itens.Add(new() { IdJogo = rd.GetInt32(0), IdGenero = rd.GetInt32(1) });
        return itens;
    }

    public async Task<IReadOnlyList<JogoGenero>> ListarPorJogosAsync(IReadOnlyCollection<int> idsJogos, CancellationToken ct = default)
    {
        var ids = idsJogos.Distinct().ToArray();
        if (ids.Length == 0) return [];

        await using var cn = await AbrirConexaoAsync(ct);
        var parametros = ids.Select((_, indice) => $"@jogo{indice}");
        var sql = "SELECT co_jogo,co_genero FROM tb_jogo_genero WHERE co_jogo IN (" + string.Join(",", parametros) + ")";
        await using var cmd = CriarComando(cn, sql);
        for (var indice = 0; indice < ids.Length; indice++)
            AdicionarParametro(cmd, $"@jogo{indice}", ids[indice]);

        await using var rd = await cmd.ExecuteReaderAsync(ct);
        var itens = new List<JogoGenero>();
        while (await rd.ReadAsync(ct)) itens.Add(new() { IdJogo = rd.GetInt32(0), IdGenero = rd.GetInt32(1) });
        return itens;
    }

    public async Task InserirAsync(int idJogo, IReadOnlyCollection<int> idsGeneros, CancellationToken ct = default)
    {
        if (idsGeneros.Count == 0) return;

        await using var cn = await AbrirConexaoAsync(ct);
        var valores = Enumerable.Range(0, idsGeneros.Count).Select(indice => $"(@jogo,@genero{indice})");
        await using var cmd = CriarComando(cn, "INSERT INTO tb_jogo_genero(co_jogo,co_genero) VALUES " + string.Join(",", valores));
        AdicionarParametro(cmd, "@jogo", idJogo);
        var indiceGenero = 0;
        foreach (var idGenero in idsGeneros)
        {
            AdicionarParametro(cmd, $"@genero{indiceGenero++}", idGenero);
        }
        await ExecutarGravacaoAsync(cmd, ct);
    }

    public async Task RemoverPorJogoAsync(int idJogo, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "DELETE FROM tb_jogo_genero WHERE co_jogo=@jogo");
        AdicionarParametro(cmd, "@jogo", idJogo);
        await ExecutarGravacaoAsync(cmd, ct);
    }
}

internal sealed class UsuarioRepository(RepositoryConfig config)
    : RepositoryBase(config), IUsuarioRepository
{
    public async Task InserirAsync(Usuario usuario, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "INSERT INTO tb_usuario(no_usuario,ds_email,ds_senha_hash,st_ativo,dt_cadastro,co_perfil) VALUES(@nome,@email,@hash,1,UTC_TIMESTAMP(),@perfil)");
        AdicionarParametro(cmd, "@nome", usuario.Nome);
        AdicionarParametro(cmd, "@email", usuario.Email);
        AdicionarParametro(cmd, "@hash", usuario.SenhaHash);
        AdicionarParametro(cmd, "@perfil", usuario.IdPerfil);
        await ExecutarGravacaoAsync(cmd, ct);
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "SELECT co_seq_usuario,no_usuario,ds_email,ds_senha_hash,st_ativo,dt_cadastro,co_perfil FROM tb_usuario WHERE ds_email=@email");
        AdicionarParametro(cmd, "@email", email);
        await using var rd = await cmd.ExecuteReaderAsync(ct);
        return await rd.ReadAsync(ct)
            ? new Usuario
            {
                Id = rd.GetInt32(0),
                Nome = rd.GetString(1),
                Email = rd.GetString(2),
                SenhaHash = rd.GetString(3),
                Ativo = rd.GetBoolean(4),
                DataCadastro = rd.GetDateTime(5),
                IdPerfil = rd.GetInt32(6),
            }
            : null;
    }
}

internal sealed class PerfilRepository(RepositoryConfig config)
    : RepositoryBase(config), IPerfilRepository
{
    public async Task<string?> ObterDescricaoAsync(int id, CancellationToken ct = default)
    {
        await using var cn = await AbrirConexaoAsync(ct);
        await using var cmd = CriarComando(cn, "SELECT ds_perfil FROM tb_perfil WHERE co_perfil=@id");
        AdicionarParametro(cmd, "@id", id);
        return (string?)await cmd.ExecuteScalarAsync(ct);
    }
}
