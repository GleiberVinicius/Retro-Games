namespace RetroGames.WebApi.ApiModels.ApiModels;
public sealed record ListarPlataformaResponseApiModel(int Id, string Nome, string Fabricante, short AnoLancamento);
public sealed class PlataformaEntradaApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public short AnoLancamento { get; set; }
}
