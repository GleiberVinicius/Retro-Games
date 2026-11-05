namespace RetroGames.Model.Model;
public sealed class Jogo
{
    public int Id { get; set; } // co_seq_jogo
    public string Nome { get; set; } = string.Empty; // no_jogo
    public string? Descricao { get; set; } // ds_jogo
    public short AnoLancamento { get; set; } // nu_ano_lancamento
    public decimal Preco { get; set; } // vl_preco
    public int Estoque { get; set; } // qt_estoque
    public bool Ativo { get; set; } // st_ativo
    public DateTime DataCadastro { get; set; } // dt_cadastro
    public int IdPlataforma { get; set; } // co_plataforma
    public int IdCondicaoJogo { get; set; } // co_condicao_jogo
}
