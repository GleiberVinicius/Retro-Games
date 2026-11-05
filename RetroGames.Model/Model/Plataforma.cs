namespace RetroGames.Model.Model;
public sealed class Plataforma
{
    public int Id { get; set; } // co_seq_plataforma
    public string Nome { get; set; } = string.Empty; // no_plataforma
    public string Fabricante { get; set; } = string.Empty; // no_fabricante
    public short AnoLancamento { get; set; } // nu_ano_lancamento
}
