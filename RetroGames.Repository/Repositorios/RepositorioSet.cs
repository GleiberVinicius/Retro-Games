using MySqlConnector;
using RetroGames.Model.Exceptions;
using RetroGames.Model.Model;
using RetroGames.Repository.Conexao;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repositorios;

public sealed class RepositorioSet(MySqlConexaoFactory conexoes) : IRepositorioSet
{
    public IPlataformaRepository Plataformas { get; } = new PlataformaRepository(conexoes);
    public IJogoRepository Jogos { get; } = new JogoRepository(conexoes);
    public IGeneroRepository Generos { get; } = new GeneroRepository(conexoes);
    public ICondicaoJogoRepository Condicoes { get; } = new CondicaoJogoRepository(conexoes);
    public IUsuarioRepository Usuarios { get; } = new UsuarioRepository(conexoes);
    public IPerfilRepository Perfis { get; } = new PerfilRepository(conexoes);
    public IJogoGeneroRepository JogosGeneros { get; } = new JogoGeneroRepository(conexoes);
}

internal static class BancoDados
{
    public static async Task ExecutarGravacaoAsync(MySqlCommand comando, CancellationToken ct)
    {
        try { await comando.ExecuteNonQueryAsync(ct); }
        catch (MySqlException ex) when (ex.Number == 1062) { throw new RegistroDuplicadoException("Registro duplicado."); }
    }
}

internal sealed class PlataformaRepository(MySqlConexaoFactory conexoes) : IPlataformaRepository
{
    public async Task<IReadOnlyList<Plataforma>> ListarAsync(CancellationToken ct = default)
    {
        await using var cn = conexoes.Criar(); await cn.OpenAsync(ct);
        await using var cmd = new MySqlCommand("SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma ORDER BY no_plataforma", cn);
        await using var rd = await cmd.ExecuteReaderAsync(ct); var itens = new List<Plataforma>();
        while (await rd.ReadAsync(ct)) itens.Add(Mapear(rd)); return itens;
    }
    public async Task<Plataforma?> ObterAsync(int id, CancellationToken ct = default)
    {
        await using var cn = conexoes.Criar(); await cn.OpenAsync(ct);
        await using var cmd = new MySqlCommand("SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma WHERE co_seq_plataforma = @id", cn); cmd.Parameters.AddWithValue("@id", id);
        await using var rd = await cmd.ExecuteReaderAsync(ct); return await rd.ReadAsync(ct) ? Mapear(rd) : null;
    }
    public async Task<bool> ExisteNomeAsync(string nome, int? ignorarId = null, CancellationToken ct = default)
    {
        await using var cn = conexoes.Criar(); await cn.OpenAsync(ct);
        var sql = "SELECT COUNT(*) FROM tb_plataforma WHERE no_plataforma = @nome" + (ignorarId.HasValue ? " AND co_seq_plataforma <> @id" : "");
        await using var cmd = new MySqlCommand(sql, cn); cmd.Parameters.AddWithValue("@nome", nome); if (ignorarId.HasValue) cmd.Parameters.AddWithValue("@id", ignorarId.Value);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)) > 0;
    }
    public async Task<int> InserirAsync(Plataforma item, CancellationToken ct = default)
    {
        await using var cn = conexoes.Criar(); await cn.OpenAsync(ct);
        await using var cmd = new MySqlCommand("INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) VALUES (@nome, @fabricante, @ano)", cn);
        cmd.Parameters.AddWithValue("@nome", item.Nome); cmd.Parameters.AddWithValue("@fabricante", item.Fabricante); cmd.Parameters.AddWithValue("@ano", item.AnoLancamento);
        await BancoDados.ExecutarGravacaoAsync(cmd,ct); return checked((int)cmd.LastInsertedId);
    }
    public async Task AtualizarAsync(Plataforma item, CancellationToken ct = default)
    {
        await using var cn = conexoes.Criar(); await cn.OpenAsync(ct);
        await using var cmd = new MySqlCommand("UPDATE tb_plataforma SET no_plataforma=@nome, no_fabricante=@fabricante, nu_ano_lancamento=@ano WHERE co_seq_plataforma=@id", cn);
        cmd.Parameters.AddWithValue("@nome", item.Nome); cmd.Parameters.AddWithValue("@fabricante", item.Fabricante); cmd.Parameters.AddWithValue("@ano", item.AnoLancamento); cmd.Parameters.AddWithValue("@id", item.Id);
        await BancoDados.ExecutarGravacaoAsync(cmd, ct);
    }
    public async Task ExcluirAsync(int id, CancellationToken ct = default) { await using var cn=conexoes.Criar(); await cn.OpenAsync(ct); await using var cmd=new MySqlCommand("DELETE FROM tb_plataforma WHERE co_seq_plataforma=@id",cn); cmd.Parameters.AddWithValue("@id",id); await cmd.ExecuteNonQueryAsync(ct); }
    private static Plataforma Mapear(MySqlDataReader rd) => new() { Id=rd.GetInt32("co_seq_plataforma"), Nome=rd.GetString("no_plataforma"), Fabricante=rd.GetString("no_fabricante"), AnoLancamento=rd.GetInt16("nu_ano_lancamento") };
}

internal sealed class JogoRepository(MySqlConexaoFactory conexoes) : IJogoRepository
{
    private static string Filtros(string? busca, int? plataforma, bool inativos, IReadOnlyCollection<int>? ids)
    {
        var partes = new List<string>(); if (!inativos) partes.Add("st_ativo=1"); if (plataforma.HasValue) partes.Add("co_plataforma=@plataforma"); if (!string.IsNullOrWhiteSpace(busca)) partes.Add("no_jogo LIKE @busca");
        if (ids is { Count: > 0 }) partes.Add("co_seq_jogo IN (" + string.Join(",", ids.Select((_, i) => "@j" + i)) + ")"); else if (ids is { Count: 0 }) partes.Add("1=0");
        return partes.Count == 0 ? "" : " WHERE " + string.Join(" AND ", partes);
    }
    private static void Parametros(MySqlCommand cmd, string? busca, int? plataforma, IReadOnlyCollection<int>? ids)
    { if (plataforma.HasValue) cmd.Parameters.AddWithValue("@plataforma", plataforma.Value); if (!string.IsNullOrWhiteSpace(busca)) cmd.Parameters.AddWithValue("@busca", "%" + busca.Trim() + "%"); if (ids is not null) for (var i=0;i<ids.Count;i++) cmd.Parameters.AddWithValue("@j"+i, ids.ElementAt(i)); }
    public async Task<IReadOnlyList<Jogo>> ListarAsync(string? busca,int? idPlataforma,bool incluirInativos,int pagina,int tamanhoPagina,IReadOnlyCollection<int>? ids,CancellationToken ct=default)
    {
        await using var cn=conexoes.Criar(); await cn.OpenAsync(ct); var sql="SELECT co_seq_jogo,no_jogo,ds_jogo,nu_ano_lancamento,vl_preco,qt_estoque,st_ativo,dt_cadastro,co_plataforma,co_condicao_jogo FROM tb_jogo"+Filtros(busca,idPlataforma,incluirInativos,ids)+" ORDER BY no_jogo LIMIT @limite OFFSET @offset";
        await using var cmd=new MySqlCommand(sql,cn); Parametros(cmd,busca,idPlataforma,ids); cmd.Parameters.AddWithValue("@limite",tamanhoPagina); cmd.Parameters.AddWithValue("@offset",(pagina-1)*tamanhoPagina);
        await using var rd=await cmd.ExecuteReaderAsync(ct); var itens=new List<Jogo>(); while(await rd.ReadAsync(ct)) itens.Add(Mapear(rd)); return itens;
    }
    public async Task<int> ContarAsync(string? busca,int? idPlataforma,bool incluirInativos,IReadOnlyCollection<int>? ids,CancellationToken ct=default)
    { await using var cn=conexoes.Criar(); await cn.OpenAsync(ct); await using var cmd=new MySqlCommand("SELECT COUNT(*) FROM tb_jogo"+Filtros(busca,idPlataforma,incluirInativos,ids),cn); Parametros(cmd,busca,idPlataforma,ids); return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct)); }
    public async Task<Jogo?> ObterAsync(int id,bool incluirInativo,CancellationToken ct=default)
    { await using var cn=conexoes.Criar(); await cn.OpenAsync(ct); var sql="SELECT co_seq_jogo,no_jogo,ds_jogo,nu_ano_lancamento,vl_preco,qt_estoque,st_ativo,dt_cadastro,co_plataforma,co_condicao_jogo FROM tb_jogo WHERE co_seq_jogo=@id"+(incluirInativo?"":" AND st_ativo=1"); await using var cmd=new MySqlCommand(sql,cn); cmd.Parameters.AddWithValue("@id",id); await using var rd=await cmd.ExecuteReaderAsync(ct); return await rd.ReadAsync(ct)?Mapear(rd):null; }
    public async Task<bool> ExisteNaPlataformaAsync(int idPlataforma,CancellationToken ct=default) { await using var cn=conexoes.Criar(); await cn.OpenAsync(ct); await using var cmd=new MySqlCommand("SELECT COUNT(*) FROM tb_jogo WHERE co_plataforma=@id",cn); cmd.Parameters.AddWithValue("@id",idPlataforma); return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct))>0; }
    public async Task<int> InserirAsync(Jogo j,CancellationToken ct=default) { await using var cn=conexoes.Criar(); await cn.OpenAsync(ct); await using var cmd=new MySqlCommand("INSERT INTO tb_jogo(no_jogo,ds_jogo,nu_ano_lancamento,vl_preco,qt_estoque,st_ativo,dt_cadastro,co_plataforma,co_condicao_jogo) VALUES(@nome,@descricao,@ano,@preco,@estoque,1,UTC_TIMESTAMP(),@plataforma,@condicao)",cn); ParametrosJogo(cmd,j); await cmd.ExecuteNonQueryAsync(ct); return checked((int)cmd.LastInsertedId); }
    public async Task AtualizarAsync(Jogo j,CancellationToken ct=default) { await using var cn=conexoes.Criar(); await cn.OpenAsync(ct); await using var cmd=new MySqlCommand("UPDATE tb_jogo SET no_jogo=@nome,ds_jogo=@descricao,nu_ano_lancamento=@ano,vl_preco=@preco,qt_estoque=@estoque,co_plataforma=@plataforma,co_condicao_jogo=@condicao WHERE co_seq_jogo=@id",cn); ParametrosJogo(cmd,j); cmd.Parameters.AddWithValue("@id",j.Id); await cmd.ExecuteNonQueryAsync(ct); }
    public async Task DesativarAsync(int id,CancellationToken ct=default) { await using var cn=conexoes.Criar(); await cn.OpenAsync(ct); await using var cmd=new MySqlCommand("UPDATE tb_jogo SET st_ativo=0 WHERE co_seq_jogo=@id",cn); cmd.Parameters.AddWithValue("@id",id); await cmd.ExecuteNonQueryAsync(ct); }
    private static void ParametrosJogo(MySqlCommand cmd,Jogo j) { cmd.Parameters.AddWithValue("@nome",j.Nome); cmd.Parameters.AddWithValue("@descricao",(object?)j.Descricao??DBNull.Value); cmd.Parameters.AddWithValue("@ano",j.AnoLancamento); cmd.Parameters.AddWithValue("@preco",j.Preco); cmd.Parameters.AddWithValue("@estoque",j.Estoque); cmd.Parameters.AddWithValue("@plataforma",j.IdPlataforma); cmd.Parameters.AddWithValue("@condicao",j.IdCondicaoJogo); }
    private static Jogo Mapear(MySqlDataReader r) => new(){ Id=r.GetInt32("co_seq_jogo"),Nome=r.GetString("no_jogo"),Descricao=r.IsDBNull(r.GetOrdinal("ds_jogo"))?null:r.GetString("ds_jogo"),AnoLancamento=r.GetInt16("nu_ano_lancamento"),Preco=r.GetDecimal("vl_preco"),Estoque=r.GetInt32("qt_estoque"),Ativo=r.GetBoolean("st_ativo"),DataCadastro=DateTime.SpecifyKind(r.GetDateTime("dt_cadastro"),DateTimeKind.Utc),IdPlataforma=r.GetInt32("co_plataforma"),IdCondicaoJogo=r.GetInt32("co_condicao_jogo")};
}

internal sealed class GeneroRepository(MySqlConexaoFactory c) : IGeneroRepository
{
    public Task<IReadOnlyList<Genero>> ListarAsync(CancellationToken ct=default)=>ConsultarAsync("SELECT co_seq_genero,no_genero FROM tb_genero ORDER BY no_genero",null,ct);
    public async Task<IReadOnlyList<Genero>> ListarPorIdsAsync(IReadOnlyCollection<int> ids,CancellationToken ct=default) { if(ids.Count==0)return []; await using var cn=c.Criar(); await cn.OpenAsync(ct); var pars=string.Join(",",ids.Select((_,i)=>"@i"+i)); await using var cmd=new MySqlCommand("SELECT co_seq_genero,no_genero FROM tb_genero WHERE co_seq_genero IN ("+pars+")",cn); for(var i=0;i<ids.Count;i++)cmd.Parameters.AddWithValue("@i"+i,ids.ElementAt(i)); await using var rd=await cmd.ExecuteReaderAsync(ct);var l=new List<Genero>();while(await rd.ReadAsync(ct))l.Add(new(){Id=rd.GetInt32(0),Nome=rd.GetString(1)});return l; }
    private async Task<IReadOnlyList<Genero>> ConsultarAsync(string sql,int? id,CancellationToken ct){ await using var cn=c.Criar();await cn.OpenAsync(ct);await using var cmd=new MySqlCommand(sql,cn);if(id.HasValue)cmd.Parameters.AddWithValue("@id",id.Value);await using var rd=await cmd.ExecuteReaderAsync(ct);var l=new List<Genero>();while(await rd.ReadAsync(ct))l.Add(new(){Id=rd.GetInt32(0),Nome=rd.GetString(1)});return l; }
}

internal sealed class CondicaoJogoRepository(MySqlConexaoFactory c) : ICondicaoJogoRepository
{
    public async Task<IReadOnlyList<CondicaoJogo>> ListarAsync(CancellationToken ct=default){await using var cn=c.Criar();await cn.OpenAsync(ct);await using var cmd=new MySqlCommand("SELECT co_condicao_jogo,ds_condicao_jogo FROM tb_condicao_jogo ORDER BY co_condicao_jogo",cn);await using var rd=await cmd.ExecuteReaderAsync(ct);var l=new List<CondicaoJogo>();while(await rd.ReadAsync(ct))l.Add(new(){Id=rd.GetInt32(0),Descricao=rd.GetString(1)});return l;}
    public async Task<bool> ExisteAsync(int id,CancellationToken ct=default){await using var cn=c.Criar();await cn.OpenAsync(ct);await using var cmd=new MySqlCommand("SELECT COUNT(*) FROM tb_condicao_jogo WHERE co_condicao_jogo=@id",cn);cmd.Parameters.AddWithValue("@id",id);return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct))>0;}
}
internal sealed class JogoGeneroRepository(MySqlConexaoFactory c) : IJogoGeneroRepository
{
    public async Task<IReadOnlyList<JogoGenero>> ListarTodosAsync(CancellationToken ct=default){await using var cn=c.Criar();await cn.OpenAsync(ct);await using var cmd=new MySqlCommand("SELECT co_jogo,co_genero FROM tb_jogo_genero",cn);await using var r=await cmd.ExecuteReaderAsync(ct);var l=new List<JogoGenero>();while(await r.ReadAsync(ct))l.Add(new(){IdJogo=r.GetInt32(0),IdGenero=r.GetInt32(1)});return l;}
    public async Task SubstituirAsync(int idJogo,IReadOnlyCollection<int> ids,CancellationToken ct=default){await using(var cn=c.Criar()){await cn.OpenAsync(ct);await using var cmd=new MySqlCommand("DELETE FROM tb_jogo_genero WHERE co_jogo=@id",cn);cmd.Parameters.AddWithValue("@id",idJogo);await cmd.ExecuteNonQueryAsync(ct);} foreach(var id in ids){await using var cn=c.Criar();await cn.OpenAsync(ct);await using var cmd=new MySqlCommand("INSERT INTO tb_jogo_genero(co_jogo,co_genero) VALUES(@jogo,@genero)",cn);cmd.Parameters.AddWithValue("@jogo",idJogo);cmd.Parameters.AddWithValue("@genero",id);await cmd.ExecuteNonQueryAsync(ct);}}
}
internal sealed class UsuarioRepository(MySqlConexaoFactory c) : IUsuarioRepository
{
    public async Task InserirAsync(Usuario u,CancellationToken ct=default){await using var cn=c.Criar();await cn.OpenAsync(ct);await using var cmd=new MySqlCommand("INSERT INTO tb_usuario(no_usuario,ds_email,ds_senha_hash,st_ativo,dt_cadastro,co_perfil) VALUES(@nome,@email,@hash,1,UTC_TIMESTAMP(),@perfil)",cn);cmd.Parameters.AddWithValue("@nome",u.Nome);cmd.Parameters.AddWithValue("@email",u.Email);cmd.Parameters.AddWithValue("@hash",u.SenhaHash);cmd.Parameters.AddWithValue("@perfil",u.IdPerfil);await BancoDados.ExecutarGravacaoAsync(cmd,ct);}
    public async Task<Usuario?> ObterPorEmailAsync(string email,CancellationToken ct=default){await using var cn=c.Criar();await cn.OpenAsync(ct);await using var cmd=new MySqlCommand("SELECT co_seq_usuario,no_usuario,ds_email,ds_senha_hash,st_ativo,dt_cadastro,co_perfil FROM tb_usuario WHERE ds_email=@email",cn);cmd.Parameters.AddWithValue("@email",email);await using var r=await cmd.ExecuteReaderAsync(ct);return await r.ReadAsync(ct)?new(){Id=r.GetInt32(0),Nome=r.GetString(1),Email=r.GetString(2),SenhaHash=r.GetString(3),Ativo=r.GetBoolean(4),DataCadastro=r.GetDateTime(5),IdPerfil=r.GetInt32(6)}:null;}
}
internal sealed class PerfilRepository(MySqlConexaoFactory c) : IPerfilRepository
{
    public async Task<string?> ObterDescricaoAsync(int id,CancellationToken ct=default){await using var cn=c.Criar();await cn.OpenAsync(ct);await using var cmd=new MySqlCommand("SELECT ds_perfil FROM tb_perfil WHERE co_perfil=@id",cn);cmd.Parameters.AddWithValue("@id",id);return (string?)await cmd.ExecuteScalarAsync(ct);}
}
